// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroElements.CodeContracts;
using MicroElements.Extensions;
using MicroElements.Functional.Internals;

namespace MicroElements.Reflection
{
    /// <summary>
    /// Represents type cache and methods for working with types.
    /// Purpose:
    /// 1. Type loading from various sources with various filters;
    /// 2. Reducing reflection time with caching by name or alias.
    /// </summary>
    /// <para>Use: <see cref="TypeCache.Create"/> method to create type cache.</para>
    public interface ITypeCache
    {
        /// <summary>
        /// Gets Assembly filters that was used to get <see cref="Assemblies"/>.
        /// </summary>
        public AssemblySource AssemblySource { get; }

        /// <summary>
        /// Gets Type filters that was used to get <see cref="Types"/>.
        /// </summary>
        public TypeSource TypeSource { get; }

        /// <summary>
        /// Gets assemblies that matches assembly filters.
        /// </summary>
        public IReadOnlyCollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets types that matches type filters.
        /// </summary>
        public IReadOnlyCollection<Type> Types { get; }

        /// <summary>
        /// Gets types indexed by <see cref="Type.FullName"/>.
        /// </summary>
        public IReadOnlyDictionary<string, Type> TypeByFullName { get; }

        /// <summary>
        /// Gets types indexed by type alias.
        /// </summary>
        public IReadOnlyDictionary<string, Type> TypeByAlias { get; }

        /// <summary>
        /// Gets type by its alias name.
        /// </summary>
        public IReadOnlyDictionary<Type, string> AliasForType { get; }
    }

    /// <summary>
    /// Default  type cache implementation.
    /// </summary>
    public class TypeCacheDefault : ITypeCache
    {
        /// <summary>
        /// Gets Assembly filters that was used to get <see cref="Assemblies"/>.
        /// </summary>
        public AssemblySource AssemblySource { get; }

        /// <summary>
        /// Gets Type filters that was used to get <see cref="Types"/>.
        /// </summary>
        public TypeSource TypeSource { get; }

        /// <summary>
        /// Gets assemblies that matches assembly filters.
        /// </summary>
        public IReadOnlyCollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets types that matches type filters.
        /// </summary>
        public IReadOnlyCollection<Type> Types { get; }

        /// <summary>
        /// Gets types indexed by <see cref="Type.FullName"/>.
        /// </summary>
        public IReadOnlyDictionary<string, Type> TypeByFullName { get; }

        /// <summary>
        /// Gets types indexed by type alias.
        /// </summary>
        public IReadOnlyDictionary<string, Type> TypeByAlias { get; }

        /// <summary>
        /// Gets type by its alias name.
        /// </summary>
        public IReadOnlyDictionary<Type, string> AliasForType { get; }

        /// <summary>
        /// Creates type cache.
        /// </summary>
        /// <param name="assemblySource">Assembly filters that was used to get <see cref="Assemblies"/>.</param>
        /// <param name="typeSource">Type filters that was used to get <see cref="Types"/>.</param>
        public TypeCacheDefault(
            AssemblySource assemblySource,
            TypeSource typeSource)
        {
            assemblySource.AssertArgumentNotNull(nameof(assemblySource));
            typeSource.AssertArgumentNotNull(nameof(typeSource));

            AssemblySource = assemblySource;
            TypeSource = typeSource;
            Assemblies = assemblySource.ResultAssemblies.NotNull().ToArray();

            Types = TypeSource
                .TypeRegistrations
                .Select(registration => registration.Type)
                .Distinct()
                .ToArray();

            string[] typeNames = Types
                .Select(type => type.FullName)
                .Distinct()
                .ToArray();

            if (typeNames.Length != Types.Count)
            {
                TypeByFullName = Types
                    .GroupBy(type => type.FullName)
                    .Select(group => group.First())
                    .ToDictionary(type => type.FullName!, type => type);
            }
            else
            {
                TypeByFullName = Types
                    .ToDictionary(type => type.FullName!, type => type);
            }

            TypeByAlias = TypeSource
                .TypeRegistrations
                .Where(registration => registration.Alias != null)
                .ToDictionary(
                    registration => registration.Alias ?? registration.Type.FullName,
                    registration => registration.Type);

            AliasForType = TypeSource
                .TypeRegistrations
                .Where(registration => registration.Alias != null)
                .ToDictionary(
                    registration => registration.Type,
                    registration => registration.Alias ?? registration.Type.Name);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            IEnumerable<string> Parts()
            {
                yield return $"AssemblyCount: {Assemblies.Count}";
                yield return $"TypeCount: {Types.Count}";
                yield return $"Assemblies: {Assemblies.Select(assembly => assembly.GetName().Name).FormatAsTuple(maxItems: 20)}";
                yield return $"Types: {TypeSource.TypeRegistrations.Select(registration => registration.Alias ?? registration.Type.FullName).FormatAsTuple(maxItems: 40)}";
            }

            return Parts().FormatAsTuple();
        }
    }

    /// <summary>
    /// Represents type cache and methods for working with types.
    /// Purpose:
    /// 1. Type loading from various sources with various filters;
    /// 2. Reducing reflection time with caching by name or alias.
    /// </summary>
    /// <para>Use: <see cref="TypeCache.Create"/> method to create type cache.</para>
    public static class TypeCache
    {
        /// <summary>
        /// Gets default type cache with all assembly types.
        /// </summary>
        public static Lazy<ITypeCache> Default { get; } = CreateLazy(AssemblySource.Default, TypeSource.AllPublicTypes);

        /// <summary>
        /// Gets Numeric types set with aliases.
        /// Types: byte, short, int, long, float, double, decimal, sbyte, ushort, uint, ulong.
        /// </summary>
        public static ITypeCache NumericTypes { get; } = Create(
            typeSource: TypeSource.Empty.With(typeRegistrations: new[]
            {
                new TypeRegistration(typeof(byte), "byte"),
                new TypeRegistration(typeof(short), "short"),
                new TypeRegistration(typeof(int), "int"),
                new TypeRegistration(typeof(long), "long"),
                new TypeRegistration(typeof(float), "float"),
                new TypeRegistration(typeof(double), "double"),
                new TypeRegistration(typeof(decimal), "decimal"),
                new TypeRegistration(typeof(sbyte), "sbyte"),
                new TypeRegistration(typeof(ushort), "ushort"),
                new TypeRegistration(typeof(uint), "uint"),
                new TypeRegistration(typeof(ulong), "ulong"),
            }));

        /// <summary>
        /// Gets Numeric types set with nullable numeric types.
        /// Types: byte, short, int, long, float, double, decimal, sbyte, ushort, uint, ulong.
        /// NullableTypes: byte?, short?, int?, long?, float?, double?, decimal?, sbyte?, ushort?, uint?, ulong?.
        /// </summary>
        public static ITypeCache NumericTypesWithNullable { get; } = Create(
            typeSource: TypeSource.Empty.With(typeRegistrations:
                NumericTypes.TypeSource.TypeRegistrations
                    .Concat(new[]
                    {
                        new TypeRegistration(typeof(byte?), "byte?"),
                        new TypeRegistration(typeof(short?), "short?"),
                        new TypeRegistration(typeof(int?), "int?"),
                        new TypeRegistration(typeof(long?), "long?"),
                        new TypeRegistration(typeof(float?), "float?"),
                        new TypeRegistration(typeof(double?), "double?"),
                        new TypeRegistration(typeof(decimal?), "decimal?"),
                        new TypeRegistration(typeof(sbyte?), "sbyte?"),
                        new TypeRegistration(typeof(ushort?), "ushort?"),
                        new TypeRegistration(typeof(uint?), "uint?"),
                        new TypeRegistration(typeof(ulong?), "ulong?"),
                    }).ToArray()));

        /// <summary>
        /// Struct types from NodaTime package.
        /// Many MicroElements packages uses NodaTime so moved it here.
        /// </summary>
        public static Lazy<ITypeCache> NodaTimeTypes { get; } = CreateLazy(
            typeSource: TypeSource.FromTypeRegistrations(
                Enumerable.Empty<TypeRegistration>()
                    .Concat(NodaTypeRegistrations("LocalDateTime"))
                    .Concat(NodaTypeRegistrations("LocalDate"))
                    .Concat(NodaTypeRegistrations("LocalTime"))
                    .Concat(NodaTypeRegistrations("Duration"))
                    .Concat(NodaTypeRegistrations("Instant"))
                    .Concat(NodaTypeRegistrations("Interval"))
                    .Concat(NodaTypeRegistrations("Offset"))
                    .Concat(NodaTypeRegistrations("OffsetDate"))
                    .Concat(NodaTypeRegistrations("OffsetDateTime"))
                    .Concat(NodaTypeRegistrations("YearMonth"))
                    .Concat(NodaTypeRegistrations("ZonedDateTime"))
                    .Concat(NodaTypeRegistrations("AnnualDate"))
                    .ToArray()));

        private static IEnumerable<TypeRegistration> NodaTypeRegistrations(string alias) =>
            TypeRegistration.TypeAndNullableTypeRegistrations($"NodaTime.{alias}", alias);

        /// <summary>
        /// Creates <see cref="ITypeCache"/> by <see cref="AssemblySource"/> and <see cref="TypeSource"/>.
        /// </summary>
        /// <param name="assemblySource">Assembly filters that was used to get <see cref="Assemblies"/>. If not set <see cref="AssemblySource.Empty"/> will be used.</param>
        /// <param name="typeSource">Type filters that was used to get <see cref="Types"/>. If not set <see cref="TypeSource.Empty"/> will be used.</param>
        /// <returns>New <see cref="ITypeCache"/> instance.</returns>
        public static ITypeCache Create(
            AssemblySource? assemblySource = null,
            TypeSource? typeSource = null)
        {
            assemblySource ??= AssemblySource.Empty;
            typeSource ??= TypeSource.Empty;

            ICollection<string> messages = new List<string>();

            Assembly[] assemblies = TypeLoader.LoadAssemblies(assemblySource, messages).ToArray();

            TypeRegistration[] typeRegistrations = TypeLoader.GetTypes(assemblies, typeSource.TypeFilters, messages)
                .Select(type => new TypeRegistration(type, source: TypeRegistration.SourceType.AssemblyScan))
                .Concat(typeSource.TypeRegistrations.NotNull())
                .ToArray();

            if (assemblySource.FilterByTypeFilters)
                assemblies = typeRegistrations.Select(registration => registration.Type).Select(type => type.Assembly).Distinct().ToArray();

            AssemblySource assemblySourceResolved = assemblySource.With(resultAssemblies: assemblies);
            TypeSource typeSourceResolved = typeSource.With(typeRegistrations: typeRegistrations);

            return new TypeCacheDefault(assemblySourceResolved, typeSourceResolved);
        }

        /// <summary>
        /// Creates Lazy <see cref="ITypeCache"/> by <see cref="AssemblySource"/> and <see cref="TypeSource"/>.
        /// </summary>
        /// <param name="assemblySource">Assembly filters that was used to get <see cref="Assemblies"/>. If not set <see cref="AssemblySource.Empty"/> will be used.</param>
        /// <param name="typeSource">Type filters that was used to get <see cref="Types"/>. If not set <see cref="TypeSource.Empty"/> will be used.</param>
        /// <returns>New Lazy <see cref="ITypeCache"/> instance.</returns>
        public static Lazy<ITypeCache> CreateLazy(
            AssemblySource? assemblySource = null,
            TypeSource? typeSource = null)
        {
            return new Lazy<ITypeCache>(() => Create(assemblySource, typeSource));
        }

        /// <summary>
        /// Creates new <see cref="ITypeCache"/> with some changes.
        /// </summary>
        /// <param name="source">Source type cache.</param>
        /// <param name="assemblySource">Optional AssemblySource.</param>
        /// <param name="typeSource">Optional TypeSource.</param>
        /// <returns>New <see cref="ITypeCache"/> instance.</returns>
        public static ITypeCache With(
            this ITypeCache source,
            AssemblySource? assemblySource = null,
            TypeSource? typeSource = null)
        {
            return new TypeCacheDefault(
                assemblySource: assemblySource ?? source.AssemblySource,
                typeSource: typeSource ?? source.TypeSource);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="ITypeCache"/>.
    /// </summary>
    public static class TypeCacheExtensions
    {
        /// <summary>
        /// Determines whether the type cache contains the specified type.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type cache contains the specified type; otherwise, false.</returns>
        public static bool Contains(this ITypeCache typeCache, Type type)
        {
            return typeCache.TypeByFullName.ContainsKey(type.FullName);
        }

        /// <summary>
        /// Determines whether the type cache contains the specified type.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="typeFullName">Type name to check.</param>
        /// <returns>true if the type cache contains the specified type; otherwise, false.</returns>
        public static bool ContainsByFullName(this ITypeCache typeCache, string typeFullName)
        {
            return typeCache.TypeByFullName.ContainsKey(typeFullName);
        }

        /// <summary>
        /// Gets type by specified <paramref name="typeFullName"/>.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="typeFullName">Type name to check.</param>
        /// <returns>true if the type cache contains the specified type; otherwise, false.</returns>
        public static Type? GetByFullName(this ITypeCache typeCache, string typeFullName)
        {
            return typeCache.TypeByFullName.GetValueOrDefault(typeFullName);
        }

        /// <summary>
        /// Gets type by specified <paramref name="alias"/>.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="alias">Type name alias.</param>
        /// <returns>true if the type cache contains the specified type; otherwise, false.</returns>
        public static Type? GetByAlias(this ITypeCache typeCache, string alias)
        {
            return typeCache.TypeByAlias.GetValueOrDefault(alias);
        }

        /// <summary>
        /// Gets optional alias for type.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="type">Type to search.</param>
        /// <returns>true if the type cache contains the specified type; otherwise, false.</returns>
        public static string? GetAliasForType(this ITypeCache typeCache, Type type)
        {
            return typeCache.AliasForType.GetValueOrDefault(type);
        }

        /// <summary>
        /// Gets type by alias or fullName from <paramref name="typeCache"/>.
        /// </summary>
        /// <param name="typeCache">Source type cache.</param>
        /// <param name="typeName">Type alias or fullName.</param>
        /// <returns>Type or null.</returns>
        public static Type? GetByAliasOrFullName(this ITypeCache typeCache, string typeName)
        {
            Type? type = typeCache.GetByAlias(typeName);
            if (type != null)
                return type;

            type = typeCache.GetByFullName(typeName);
            if (type != null)
                return type;

            return Type.GetType(typeName);
        }
    }
}
