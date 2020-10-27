/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

// ReSharper disable InheritdocConsiderUsage

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace

namespace JetBrains.Annotations
{
    /// <summary>
    /// Indicates that the marked method builds string by the format pattern and (optional) arguments.
    /// The parameter, which contains the format string, should be given in constructor. The format string
    /// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form.
    /// </summary>
    /// <example><code>
    /// [StringFormatMethod("message")]
    /// void ShowError(string message, params object[] args) { /* do something */ }
    /// 
    /// void Foo() {
    ///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
    /// }
    /// </code></example>
    [AttributeUsage(
      AttributeTargets.Constructor | AttributeTargets.Method |
      AttributeTargets.Property | AttributeTargets.Delegate)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
        /// <param name="formatParameterName">
        /// Specifies which parameter of an annotated method should be treated as the format string
        /// </param>
        public StringFormatMethodAttribute([NotNull] string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        [NotNull] public string FormatParameterName { get; }
    }

    /// <summary>
    /// Indicates that the function argument should be a string literal and match one
    /// of the parameters of the caller function. For example, ReSharper annotates
    /// the parameter of <see cref="System.ArgumentNullException"/>.
    /// </summary>
    /// <example><code>
    /// void Foo(string param) {
    ///   if (param == null)
    ///     throw new ArgumentNullException("par"); // Warning: Cannot resolve symbol
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class InvokerParameterNameAttribute : Attribute { }

    /// <summary>
    /// Describes dependency between method input and output.
    /// </summary>
    /// <syntax>
    /// <p>Function Definition Table syntax:</p>
    /// <list>
    /// <item>FDT      ::= FDTRow [;FDTRow]*</item>
    /// <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
    /// <item>Input    ::= ParameterName: Value [, Input]*</item>
    /// <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
    /// <item>Value    ::= true | false | null | notnull | canbenull</item>
    /// </list>
    /// If the method has a single input parameter, its name could be omitted.<br/>
    /// Using <c>halt</c> (or <c>void</c>/<c>nothing</c>, which is the same) for the method output
    /// means that the method doesn't return normally (throws or terminates the process).<br/>
    /// Value <c>canbenull</c> is only applicable for output parameters.<br/>
    /// You can use multiple <c>[ContractAnnotation]</c> for each FDT row, or use single attribute
    /// with rows separated by semicolon. There is no notion of order rows, all rows are checked
    /// for applicability and applied per each program state tracked by the analysis engine.<br/>
    /// </syntax>
    /// <examples><list>
    /// <item><code>
    /// [ContractAnnotation("=&gt; halt")]
    /// public void TerminationMethod()
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("null &lt;= param:null")] // reverse condition syntax
    /// public string GetName(string surname)
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("s:null =&gt; true")]
    /// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
    /// </code></item>
    /// <item><code>
    /// // A method that returns null if the parameter is null,
    /// // and not null if the parameter is not null
    /// [ContractAnnotation("null =&gt; null; notnull =&gt; notnull")]
    /// public object Transform(object data)
    /// </code></item>
    /// <item><code>
    /// [ContractAnnotation("=&gt; true, result: notnull; =&gt; false, result: null")]
    /// public bool TryParse(string s, out Person result)
    /// </code></item>
    /// </list></examples>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class ContractAnnotationAttribute : Attribute
    {
        public ContractAnnotationAttribute([NotNull] string contract)
          : this(contract, false) { }

        public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
        {
            Contract = contract;
            ForceFullStates = forceFullStates;
        }

        [NotNull] public string Contract { get; }

        public bool ForceFullStates { get; }
    }

    /// <summary>
    /// When applied to a target attribute, specifies a requirement for any type marked
    /// with the target attribute to implement or inherit specific type or types.
    /// </summary>
    /// <example><code>
    /// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
    /// class ComponentAttribute : Attribute { }
    /// 
    /// [Component] // ComponentAttribute requires implementing IComponent interface
    /// class MyComponent : IComponent { }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [BaseTypeRequired(typeof(Attribute))]
    internal sealed class BaseTypeRequiredAttribute : Attribute
    {
        public BaseTypeRequiredAttribute([NotNull] Type baseType)
        {
            BaseType = baseType;
        }

        [NotNull] public Type BaseType { get; }
    }

    /// <summary>
    /// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
    /// so this symbol will not be reported as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class UsedImplicitlyAttribute : Attribute
    {
        public UsedImplicitlyAttribute()
          : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

        public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
          : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

        public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
          : this(ImplicitUseKindFlags.Default, targetFlags) { }

        public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        public ImplicitUseKindFlags UseKindFlags { get; }

        public ImplicitUseTargetFlags TargetFlags { get; }
    }

    /// <summary>
    /// Can be applied to attributes, type parameters, and parameters of a type assignable from <see cref="System.Type"/> .
    /// When applied to an attribute, the decorated attribute behaves the same as <see cref="UsedImplicitlyAttribute"/>.
    /// When applied to a type parameter or to a parameter of type <see cref="System.Type"/>,  indicates that the corresponding type
    /// is used implicitly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
    internal sealed class MeansImplicitUseAttribute : Attribute
    {
        public MeansImplicitUseAttribute()
          : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
          : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

        public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
          : this(ImplicitUseKindFlags.Default, targetFlags) { }

        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        [UsedImplicitly] public ImplicitUseKindFlags UseKindFlags { get; }

        [UsedImplicitly] public ImplicitUseTargetFlags TargetFlags { get; }
    }

    /// <summary>
    /// Specify the details of implicitly used symbol when it is marked
    /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    internal enum ImplicitUseKindFlags
    {
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
        /// <summary>Only entity marked with attribute considered used.</summary>
        Access = 1,
        /// <summary>Indicates implicit assignment to a member.</summary>
        Assign = 2,
        /// <summary>
        /// Indicates implicit instantiation of a type with fixed constructor signature.
        /// That means any unused constructor parameters won't be reported as such.
        /// </summary>
        InstantiatedWithFixedConstructorSignature = 4,
        /// <summary>Indicates implicit instantiation of a type.</summary>
        InstantiatedNoFixedConstructorSignature = 8,
    }

    /// <summary>
    /// Specify what is considered to be used implicitly when marked
    /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        Default = Itself,
        Itself = 1,
        /// <summary>Members of entity marked with attribute are considered used.</summary>
        Members = 2,
        /// <summary>Entity marked with attribute and all its members considered used.</summary>
        WithMembers = Itself | Members
    }

    /// <summary>
    /// This attribute is intended to mark publicly available API
    /// which should not be removed and so is treated as used.
    /// </summary>
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class PublicAPIAttribute : Attribute
    {
        public PublicAPIAttribute() { }

        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        [MaybeNull] public string Comment { get; }
    }

    /// <summary>
    /// Indicates that the return value of the method invocation must be used.
    /// </summary>
    /// <remarks>
    /// Methods decorated with this attribute (in contrast to pure methods) might change state,
    /// but make no sense without using their return value. <br/>
    /// Similarly to <see cref="PureAttribute"/>, this attribute
    /// will help detecting usages of the method when the return value in not used.
    /// Additionally, you can optionally specify a custom message, which will be used when showing warnings, e.g.
    /// <code>[MustUseReturnValue("Use the return value to...")]</code>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MustUseReturnValueAttribute : Attribute
    {
        public MustUseReturnValueAttribute() { }

        public MustUseReturnValueAttribute([NotNull] string justification)
        {
            Justification = justification;
        }

        [MaybeNull] public string Justification { get; }
    }

    /// <summary>
    /// Indicates the type member or parameter of some type, that should be used instead of all other ways
    /// to get the value of that type. This annotation is useful when you have some "context" value evaluated
    /// and stored somewhere, meaning that all other ways to get this value must be consolidated with existing one.
    /// </summary>
    /// <example><code>
    /// class Foo {
    ///   [ProvidesContext] IBarService _barService = ...;
    /// 
    ///   void ProcessNode(INode node) {
    ///     DoSomething(node, node.GetGlobalServices().Bar);
    ///     //              ^ Warning: use value of '_barService' field
    ///   }
    /// }
    /// </code></example>
    [AttributeUsage(
      AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Method |
      AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.GenericParameter)]
    internal sealed class ProvidesContextAttribute : Attribute { }

    /// <summary>
    /// Indicates that a parameter is a path to a file or a folder within a web project.
    /// Path can be relative or absolute, starting from web root (~).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class PathReferenceAttribute : Attribute
    {
        public PathReferenceAttribute() { }

        public PathReferenceAttribute([NotNull, PathReference] string basePath)
        {
            BasePath = basePath;
        }

        [MaybeNull] public string BasePath { get; }
    }

    /// <summary>
    /// Indicates how method, constructor invocation, or property access
    /// over collection type affects the contents of the collection.
    /// Use <see cref="CollectionAccessType"/> to specify the access type.
    /// </summary>
    /// <remarks>
    /// Using this attribute only makes sense if all collection methods are marked with this attribute.
    /// </remarks>
    /// <example><code>
    /// public class MyStringCollection : List&lt;string&gt;
    /// {
    ///   [CollectionAccess(CollectionAccessType.Read)]
    ///   public string GetFirstString()
    ///   {
    ///     return this.ElementAt(0);
    ///   }
    /// }
    /// class Test
    /// {
    ///   public void Foo()
    ///   {
    ///     // Warning: Contents of the collection is never updated
    ///     var col = new MyStringCollection();
    ///     string x = col.GetFirstString();
    ///   }
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
    internal sealed class CollectionAccessAttribute : Attribute
    {
        public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
        {
            CollectionAccessType = collectionAccessType;
        }

        public CollectionAccessType CollectionAccessType { get; }
    }

    /// <summary>
    /// Provides a value for the <see cref="CollectionAccessAttribute"/> to define
    /// how the collection method invocation affects the contents of the collection.
    /// </summary>
    [Flags]
    internal enum CollectionAccessType
    {
        /// <summary>Method does not use or modify content of the collection.</summary>
        None = 0,
        /// <summary>Method only reads content of the collection but does not modify it.</summary>
        Read = 1,
        /// <summary>Method can change content of the collection but does not add new elements.</summary>
        ModifyExistingContent = 2,
        /// <summary>Method can add new elements to the collection.</summary>
        UpdatedContent = ModifyExistingContent | 4
    }

    /// <summary>
    /// Indicates that the marked method is assertion method, i.e. it halts the control flow if
    /// one of the conditions is satisfied. To set the condition, mark one of the parameters with
    /// <see cref="AssertionConditionAttribute"/> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class AssertionMethodAttribute : Attribute { }

    /// <summary>
    /// Indicates the condition parameter of the assertion method. The method itself should be
    /// marked by <see cref="AssertionMethodAttribute"/> attribute. The mandatory argument of
    /// the attribute is the assertion type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AssertionConditionAttribute : Attribute
    {
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            ConditionType = conditionType;
        }

        public AssertionConditionType ConditionType { get; }
    }

    /// <summary>
    /// Specifies assertion type. If the assertion method argument satisfies the condition,
    /// then the execution continues. Otherwise, execution is assumed to be halted.
    /// </summary>
    internal enum AssertionConditionType
    {
        /// <summary>Marked parameter should be evaluated to true.</summary>
        IS_TRUE = 0,
        /// <summary>Marked parameter should be evaluated to false.</summary>
        IS_FALSE = 1,
        /// <summary>Marked parameter should be evaluated to null value.</summary>
        IS_NULL = 2,
        /// <summary>Marked parameter should be evaluated to not null value.</summary>
        IS_NOT_NULL = 3,
    }

    /// <summary>
    /// Indicates that method is pure LINQ method, with postponed enumeration (like Enumerable.Select,
    /// .Where). This annotation allows inference of [InstantHandle] annotation for parameters
    /// of delegate type by analyzing LINQ method chains.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class LinqTunnelAttribute : Attribute { }

    /// <summary>
    /// Indicates that IEnumerable passed as a parameter is not enumerated.
    /// Use this annotation to suppress the 'Possible multiple enumeration of IEnumerable' inspection.
    /// </summary>
    /// <example><code>
    /// static void ThrowIfNull&lt;T&gt;([NoEnumeration] T v, string n) where T : class
    /// {
    ///   // custom check for null but no enumeration
    /// }
    /// 
    /// void Foo(IEnumerable&lt;string&gt; values)
    /// {
    ///   ThrowIfNull(values, nameof(values));
    ///   var x = values.ToList(); // No warnings about multiple enumeration
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NoEnumerationAttribute : Attribute { }

    /// <summary>
    /// Indicates that the marked parameter is a regular expression pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class RegexPatternAttribute : Attribute { }

    /// <summary>
    /// Prevents the Member Reordering feature from tossing members of the marked class.
    /// </summary>
    /// <remarks>
    /// The attribute must be mentioned in your member reordering patterns.
    /// </remarks>
    [AttributeUsage(
      AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum)]
    internal sealed class NoReorderAttribute : Attribute { }
}
