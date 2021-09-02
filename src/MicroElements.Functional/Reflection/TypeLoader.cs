// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MicroElements.CodeContracts;
using MicroElements.Functional;
using MicroElements.Functional.Internals;

namespace MicroElements.Reflection
{
    /// <summary>
    /// Reflection utils.
    /// </summary>
    public static class TypeLoader
    {
        /// <summary>
        /// Loads assemblies according <paramref name="assemblySource"/>.
        /// 1. Gets all assemblies from <see cref="AppDomain.CurrentDomain"/> if <see cref="AssemblySource.LoadFromDomain"/> is true.
        /// 2. Applies filters <see cref="AssemblyFilters.IncludePatterns"/> and <see cref="AssemblyFilters.ExcludePatterns"/>.
        /// 3. Optionally loads assemblies from <see cref="AssemblySource.LoadFromDirectory"/> with the same filters.
        /// </summary>
        /// <param name="assemblySource">Filters for getting and filtering assembly list.</param>
        /// <param name="messages">Message list for diagnostic messages.</param>
        /// <returns>Assemblies.</returns>
        public static IEnumerable<Assembly> LoadAssemblies(
            AssemblySource assemblySource,
            ICollection<string>? messages = null)
        {
            assemblySource.AssertArgumentNotNull(nameof(assemblySource));

            IEnumerable<Assembly> assemblies = Array.Empty<Assembly>();
            AssemblyFilters assemblyFilters = assemblySource.AssemblyFilters;

            if (assemblySource.LoadFromDomain)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            assemblies = assemblies
                .IncludeByPatterns(assembly => assembly.FullName, assemblyFilters.IncludePatterns)
                .ExcludeByPatterns(assembly => assembly.FullName, assemblyFilters.ExcludePatterns);

            if (assemblySource.LoadFromDirectory != null)
            {
                if (!Directory.Exists(assemblySource.LoadFromDirectory))
                    throw new DirectoryNotFoundException($"Assembly ScanDirectory {assemblySource.LoadFromDirectory} is not exists.");

                var assembliesFromDirectory =
                    Directory
                        .EnumerateFiles(assemblySource.LoadFromDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                        .Concat(Directory.EnumerateFiles(assemblySource.LoadFromDirectory, "*.exe", SearchOption.TopDirectoryOnly))
                        .IncludeByPatterns(fileName => fileName, assemblyFilters.IncludePatterns)
                        .ExcludeByPatterns(fileName => fileName, assemblyFilters.ExcludePatterns)
                        .Select(assemblyFile => TypeLoader.TryLoadAssemblyFrom(assemblyFile, messages)!)
                        .Where(assembly => assembly != null);

                assemblies = assemblies.Concat(assembliesFromDirectory);
            }

            assemblies = assemblies.Distinct();

            return assemblies;
        }

        /// <summary>
        /// Gets types from assembly list according type filters.
        /// </summary>
        /// <param name="assemblies">Assembly list.</param>
        /// <param name="typeFilters">Type filters.</param>
        /// <param name="messages">Message list for diagnostic messages.</param>
        /// <returns>Types that matches filters.</returns>
        public static IReadOnlyCollection<Type> GetTypes(
            IReadOnlyCollection<Assembly> assemblies,
            TypeFilters typeFilters,
            ICollection<string>? messages = null)
        {
            assemblies.AssertArgumentNotNull(nameof(assemblies));

            var types = assemblies
                .SelectMany(assembly => assembly.GetDefinedTypesSafe(messages))
                .Where(type => type.FullName != null)
                .Where(type => type.IsPublic == typeFilters.IsPublic)
                .IncludeByPatterns(type => type.FullName, typeFilters.FullNameIncludes)
                .ExcludeByPatterns(type => type.FullName, typeFilters.FullNameExcludes)
                .ToArray();

            return types;
        }

        /// <summary>
        /// Safely returns the set of loadable types from an assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="T:System.Reflection.Assembly" /> from which to load types.</param>
        /// <param name="messages">Message list for diagnostic messages.</param>
        /// <returns>
        /// The set of types from the <paramref name="assembly" />, or the subset
        /// of types that could be loaded if there was any error.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if <paramref name="assembly" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<Type> GetDefinedTypesSafe(this Assembly assembly, ICollection<string>? messages = null)
        {
            assembly.AssertArgumentNotNull(nameof(assembly));

            try
            {
                return assembly.DefinedTypes.Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (messages != null)
                {
                    foreach (Exception loaderException in ex.LoaderExceptions)
                    {
                        messages.Add(loaderException.Message);
                    }
                }

                return ex.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Tries to load assembly from file.
        /// </summary>
        /// <param name="assemblyFile">The name or path of the file that contains the manifest of the assembly.</param>
        /// <param name="messages">Message list for diagnostic messages.</param>
        /// <returns>Assembly or null if error occurred.</returns>
        public static Assembly? TryLoadAssemblyFrom(string assemblyFile, ICollection<string>? messages = null)
        {
            try
            {
                return Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception e)
            {
                messages?.Add($"Error on load assembly {assemblyFile}. Message: {e.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Assembly filters.
    /// </summary>
    public class AssemblyFilters : IFormattableObject
    {
        /// <summary>
        /// Empty assembly filter with no filtering.
        /// </summary>
        public static AssemblyFilters Empty { get; } = new AssemblyFilters(
            includePatterns: null,
            excludePatterns: null);

        /// <summary>
        /// <see cref="Assembly.FullName"/> wildcard include patterns.
        /// <example>MyCompany.*</example>
        /// </summary>
        public IReadOnlyCollection<string>? IncludePatterns { get; }

        /// <summary>
        /// <see cref="Assembly.FullName"/> wildcard exclude patterns.
        /// <example>System.*</example>
        /// </summary>
        public IReadOnlyCollection<string>? ExcludePatterns { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySource"/> class.
        /// </summary>
        /// <param name="includePatterns"><see cref="Assembly.FullName"/> wildcard include patterns.</param>
        /// <param name="excludePatterns"><see cref="Assembly.FullName"/> wildcard exclude patterns.</param>
        public AssemblyFilters(
            IReadOnlyCollection<string>? includePatterns = null,
            IReadOnlyCollection<string>? excludePatterns = null)
        {
            IncludePatterns = includePatterns;
            ExcludePatterns = excludePatterns;
        }

        /// <inheritdoc />
        public IEnumerable<(string Name, object? Value)> GetNameValuePairs()
        {
            yield return (nameof(IncludePatterns), IncludePatterns.NotNull().FormatAsTuple());
            yield return (nameof(ExcludePatterns), ExcludePatterns.NotNull().FormatAsTuple());
        }

        /// <inheritdoc />
        public override string ToString() => GetNameValuePairs().FormatAsTuple();
    }

    /// <summary>
    /// Assembly source.
    /// </summary>
    public class AssemblySource : IFormattableObject
    {
        /// <summary>
        /// Gets empty assembly source. No assemblies, no filters.
        /// </summary>
        public static AssemblySource Empty { get; } = new AssemblySource(
            loadFromDomain: false,
            loadFromDirectory: null,
            assemblyFilters: AssemblyFilters.Empty);

        /// <summary>
        /// All assemblies from AppDomain.
        /// </summary>
        public static AssemblySource Default { get; } = new AssemblySource(
            loadFromDomain: true,
            loadFromDirectory: null,
            assemblyFilters: AssemblyFilters.Empty,
            filterByTypeFilters: true);

        /// <summary>
        /// Load assemblies from <see cref="AppDomain.CurrentDomain"/>.
        /// </summary>
        public bool LoadFromDomain { get; }

        /// <summary>
        /// Optional load assemblies from provided directory.
        /// </summary>
        public string? LoadFromDirectory { get; }

        /// <summary>
        /// Filters to filter assemblies.
        /// </summary>
        public AssemblyFilters AssemblyFilters { get; }

        /// <summary>
        /// Take user provided assemblies.
        /// </summary>
        public IReadOnlyCollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Filter assemblies after type filtering and take only assemblies that owns filtered types.
        /// </summary>
        public bool FilterByTypeFilters { get; }

        /// <summary>
        /// Result assemblies.
        /// </summary>
        public IReadOnlyCollection<Assembly>? ResultAssemblies { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySource"/> class.
        /// </summary>
        /// <param name="loadFromDomain">Optional load assemblies from <see cref="AppDomain.CurrentDomain"/>.</param>
        /// <param name="loadFromDirectory">Optional load assemblies from provided directory.</param>
        /// <param name="assemblyFilters">Optional assembly filters.</param>
        /// <param name="assemblies">User provided assemblies.</param>
        /// <param name="filterByTypeFilters">Filter assemblies after type filtering and take only assemblies that owns filtered types.</param>
        /// <param name="resultAssemblies">Result assemblies.</param>
        public AssemblySource(
            bool loadFromDomain = false,
            string? loadFromDirectory = null,
            AssemblyFilters? assemblyFilters = null,
            IReadOnlyCollection<Assembly>? assemblies = null,
            bool filterByTypeFilters = true,
            IReadOnlyCollection<Assembly>? resultAssemblies = null)
        {
            LoadFromDomain = loadFromDomain;
            LoadFromDirectory = loadFromDirectory;
            AssemblyFilters = assemblyFilters ?? AssemblyFilters.Empty;
            Assemblies = assemblies ?? Array.Empty<Assembly>();
            FilterByTypeFilters = filterByTypeFilters;
            ResultAssemblies = resultAssemblies;
        }

        /// <summary>
        /// Create copy of current object with some changes.
        /// </summary>
        /// <param name="assemblyFilters">Optional change <see cref="AssemblyFilters"/>.</param>
        /// <param name="assemblies">Optional change <see cref="Assemblies"/>.</param>
        /// <param name="filterByTypeFilters">Optional change <see cref="FilterByTypeFilters"/> flag.</param>
        /// <param name="resultAssemblies">Optional change <see cref="ResultAssemblies"/>.</param>
        /// <returns>New instance of <see cref="AssemblySource"/> with changes.</returns>
        public AssemblySource With(
            AssemblyFilters? assemblyFilters = null,
            IReadOnlyCollection<Assembly>? assemblies = null,
            bool? filterByTypeFilters = null,
            IReadOnlyCollection<Assembly>? resultAssemblies = null)
        {
            AssemblySource source = this;
            return new AssemblySource(
                assemblyFilters: assemblyFilters ?? source.AssemblyFilters,
                assemblies: assemblies ?? source.Assemblies,
                filterByTypeFilters: filterByTypeFilters ?? source.FilterByTypeFilters,
                resultAssemblies: resultAssemblies ?? source.ResultAssemblies);
        }

        /// <inheritdoc />
        public IEnumerable<(string Name, object? Value)> GetNameValuePairs()
        {
            yield return (nameof(LoadFromDomain), LoadFromDomain);
            yield return (nameof(LoadFromDirectory), LoadFromDirectory);
            yield return (nameof(AssemblyFilters), AssemblyFilters);
            yield return (nameof(Assemblies), Assemblies);
            yield return (nameof(FilterByTypeFilters), FilterByTypeFilters);
            yield return (nameof(ResultAssemblies), ResultAssemblies?.Select(assembly => assembly.GetName().Name));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetNameValuePairs().FormatAsTuple();
        }
    }

    /// <summary>
    /// Type filters.
    /// </summary>
    public class TypeFilters : IFormattableObject
    {
        /// <summary>
        /// Empty type filter.
        /// </summary>
        public static TypeFilters Empty { get; } = new TypeFilters(
            isPublic: true,
            fullNameIncludes: null,
            fullNameExcludes: null);

        /// <summary>
        /// All public types excluding anonymous.
        /// </summary>
        public static TypeFilters AllPublicTypes { get; } = new TypeFilters(
            isPublic: true,
            fullNameExcludes: new[] { "<*" });

        /// <summary>
        /// Include only public types.
        /// </summary>
        public bool IsPublic { get; }

        /// <summary>
        /// Include types that <see cref="Type.FullName"/> matches filters.
        /// </summary>
        public IReadOnlyCollection<string>? FullNameIncludes { get; }

        /// <summary>
        /// Exclude types that <see cref="Type.FullName"/> matches filters.
        /// </summary>
        public IReadOnlyCollection<string>? FullNameExcludes { get; }

        /// <summary>
        /// Creates new instance of <see cref="TypeSource"/> class.
        /// </summary>
        /// <param name="isPublic">Include only public types.</param>
        /// <param name="fullNameIncludes">Include types that <see cref="Type.FullName"/> matches filters.</param>
        /// <param name="fullNameExcludes">Exclude types that <see cref="Type.FullName"/> matches filters.</param>
        public TypeFilters(
            bool isPublic = true,
            IReadOnlyCollection<string>? fullNameIncludes = null,
            IReadOnlyCollection<string>? fullNameExcludes = null)
        {
            IsPublic = isPublic;
            FullNameIncludes = fullNameIncludes;
            FullNameExcludes = fullNameExcludes;
        }

        /// <summary>
        /// Create copy of current object with some changes.
        /// </summary>
        /// <param name="isPublic">Optional change <see cref="IsPublic"/>.</param>
        /// <param name="fullNameIncludes">Optional change <see cref="FullNameIncludes"/>.</param>
        /// <param name="fullNameExcludes">Optional change <see cref="FullNameExcludes"/>.</param>
        /// <returns>New instance of <see cref="TypeFilters"/> with changes.</returns>
        public TypeFilters With(
            bool? isPublic = null,
            IReadOnlyCollection<string>? fullNameIncludes = null,
            IReadOnlyCollection<string>? fullNameExcludes = null)
        {
            TypeFilters source = this;
            return new TypeFilters(
                isPublic: isPublic ?? source.IsPublic,
                fullNameIncludes: fullNameIncludes ?? source.FullNameIncludes,
                fullNameExcludes: fullNameExcludes ?? source.FullNameExcludes);
        }

        /// <inheritdoc />
        public IEnumerable<(string Name, object? Value)> GetNameValuePairs()
        {
            yield return (nameof(IsPublic), IsPublic);
            yield return (nameof(FullNameIncludes), FullNameIncludes.NotNull().FormatAsTuple());
            yield return (nameof(FullNameExcludes), FullNameExcludes.NotNull().FormatAsTuple());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetNameValuePairs().FormatAsTuple();
        }
    }

    /// <summary>
    /// Type source.
    /// </summary>
    public class TypeSource : IFormattableObject
    {
        /// <summary>
        /// Empty type source.
        /// </summary>
        public static TypeSource Empty { get; } = new TypeSource(
            typeFilters: TypeFilters.Empty,
            typeRegistrations: null);

        /// <summary>
        /// All public types excluding anonymous.
        /// </summary>
        public static TypeSource AllPublicTypes { get; } = new TypeSource(
            typeFilters: TypeFilters.AllPublicTypes,
            typeRegistrations: null);

        /// <summary>
        /// Type filters.
        /// </summary>
        public TypeFilters TypeFilters { get; }

        /// <summary>
        /// Defined type registrations.
        /// </summary>
        public IReadOnlyCollection<TypeRegistration> TypeRegistrations { get; }

        /// <summary>
        /// Creates new instance of <see cref="TypeSource"/> class.
        /// </summary>
        /// <param name="typeFilters">Filters to filter types.</param>
        /// <param name="typeRegistrations">User provided registrations.</param>
        public TypeSource(
            TypeFilters typeFilters,
            IReadOnlyCollection<TypeRegistration>? typeRegistrations = null)
        {
            TypeRegistrations = typeRegistrations ?? Array.Empty<TypeRegistration>();
            TypeFilters = typeFilters;
        }

        /// <summary>
        /// Creates new <see cref="TypeSource"/> from provided types.
        /// </summary>
        /// <param name="types">Types to add to source.</param>
        /// <returns>New <see cref="TypeSource"/> instance.</returns>
        public static TypeSource FromTypes(params Type[] types)
        {
            TypeRegistration[] typeRegistrations = types.NotNull().Select(type => new TypeRegistration(type, source: TypeRegistration.SourceType.Manual)).ToArray();
            return Empty.With(typeRegistrations: typeRegistrations);
        }

        /// <summary>
        /// Creates new <see cref="TypeSource"/> from provided type registrations.
        /// </summary>
        /// <param name="typeRegistrations">Type registrations to add to source.</param>
        /// <returns>New <see cref="TypeSource"/> instance.</returns>
        public static TypeSource FromTypeRegistrations(params TypeRegistration[] typeRegistrations)
        {
            return Empty.With(typeRegistrations: typeRegistrations);
        }

        /// <summary>
        /// Creates copy of current object with changes.
        /// </summary>
        /// <param name="typeFilters">Optional type filters.</param>
        /// <param name="typeRegistrations">Optional type registrations.</param>
        /// <returns>New <see cref="TypeSource"/> instance.</returns>
        public TypeSource With(
            TypeFilters? typeFilters = null,
            IReadOnlyCollection<TypeRegistration>? typeRegistrations = null)
        {
            TypeSource source = this;
            return new TypeSource(
                typeFilters: typeFilters ?? source.TypeFilters,
                typeRegistrations: typeRegistrations ?? source.TypeRegistrations);
        }

        /// <inheritdoc />
        public IEnumerable<(string Name, object? Value)> GetNameValuePairs()
        {
            yield return (nameof(TypeFilters), TypeFilters);
            yield return (nameof(TypeRegistrations), TypeRegistrations);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetNameValuePairs().FormatAsTuple();
        }
    }

    /// <summary>
    /// Allows to register type in cache without assembly scanning.
    /// Allows to register type alias.
    /// </summary>
    public class TypeRegistration : ValueObject
    {
        /// <summary>
        /// Type source.
        /// </summary>
        public enum SourceType
        {
            /// <summary>
            /// Type was found on assembly scan.
            /// </summary>
            AssemblyScan,

            /// <summary>
            /// Type was registered by user.
            /// </summary>
            Manual,
        }

        /// <summary>
        /// Type that should be added to cache.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Optional type alias.
        /// For example for type 'System.Int32' set alias 'int'.
        /// </summary>
        public string? Alias { get; }

        /// <summary>
        /// Type source.
        /// </summary>
        public SourceType Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRegistration"/> class.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="source">Type source.</param>
        /// <param name="alias">Optional type alias.</param>
        public TypeRegistration(Type type, string? alias = null, SourceType source = SourceType.Manual)
        {
            type.AssertArgumentNotNull(nameof(type));

            Type = type;
            Alias = alias;
            Source = source;
        }

        /// <inheritdoc />
        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Type;
            yield return Alias;
        }

        /// <summary>
        /// Returns <see cref="TypeRegistration"/> by type FullName with optional alias.
        /// Also returns nullable type registration for this type if its a value type.
        /// </summary>
        /// <param name="fullName">Type FullName.</param>
        /// <param name="alias">Type alias.</param>
        /// <param name="typeCache">Optional type cache which contains searching type. If not set then <see cref="TypeCache.Default"/> will be used.</param>
        /// <returns>Enumeration of <see cref="TypeRegistration"/>. Can be empty if type was not found.</returns>
        public static IEnumerable<TypeRegistration> TypeAndNullableTypeRegistrations(string fullName, string? alias = null, ITypeCache? typeCache = null)
        {
            Type? type = (typeCache ?? TypeCache.Default.Value).GetByAliasOrFullName(fullName);
            if (type != null)
            {
                yield return new TypeRegistration(type, alias);
                if (type.IsValueType)
                {
                    Type nullableType = typeof(Nullable<>).MakeGenericType(type);
                    yield return new TypeRegistration(nullableType, alias + "?");
                }
            }
        }
    }

    /// <summary>
    /// Provides methods for filtering.
    /// </summary>
    internal static class Filtering
    {
        internal static string WildcardToRegex(string pat) => "^" + Regex.Escape(pat).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

        internal static bool FileNameMatchesPattern(string filename, string pattern) => Regex.IsMatch(Path.GetFileName(filename) ?? string.Empty, WildcardToRegex(pattern));

        internal static IEnumerable<T> IncludeByPatterns<T>(this IEnumerable<T> values, Func<T, string> filterComponent, IReadOnlyCollection<string>? includePatterns = null)
        {
            if (includePatterns == null)
                return values;
            return values.Where(value => includePatterns.Any(pattern => FileNameMatchesPattern(filterComponent(value), pattern)));
        }

        internal static IEnumerable<T> ExcludeByPatterns<T>(this IEnumerable<T> values, Func<T, string> filterComponent, IReadOnlyCollection<string>? excludePatterns = null)
        {
            if (excludePatterns == null)
                return values;
            return values.Where(value => excludePatterns.Any(excludePattern => !FileNameMatchesPattern(filterComponent(value), excludePattern)));
        }
    }
}
