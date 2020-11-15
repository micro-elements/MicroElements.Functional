using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    /// <summary>
    /// Question: https://stackoverflow.com/questions/58863932/how-to-mark-a-generic-delegate-to-accept-null-values
    /// </summary>
    public class NullabilityInDelegates
    {
        /// <summary>
        /// Original code from question. 2 errors.
        /// </summary>
        public void Test<TIn, TOut>(Func<TIn, TOut> func)
        {
            var result = func(default); // Error CS8604: Possible null reference argument.
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference.
        }

        /// <summary>
        /// Mark `TIn` as CanBeNull, Mark `TOut` as CanBeNull.
        /// </summary>
        public void Test2<TIn, TOut>(Func<CanBeNull<TIn>, CanBeNull<TOut>> func)
        {
            CanBeNull<TOut> canBeNull = func(new CanBeNull<TIn>(default)); // no error: null allowed
            TOut result = canBeNull.Value; // `Value` may be null here
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference.
        }

        /// <summary>
        /// Mark `TIn` as CanBeNull, Mark `TOut` as CanBeNull.
        /// Use implicit conversion. Initial sample can be used with minimal rewrite.
        /// </summary>
        public void Test3<TIn, TOut>(Func<CanBeNull<TIn>, CanBeNull<TOut>> func)
        {
            // Implicitly casts `default` to `CanBeNull<TIn>`, implicitly casts `CanBeNull<TOut>` to `TOut`
            TOut result = func(default); // No error: null allowed, 
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference.
        }

        /// <summary>
        /// Mark `TIn` as CanBeNull, `TOut` as is.
        /// </summary>
        public void Test4<TIn, TOut>(Func<CanBeNull<TIn>, TOut> func)
        {
            // Implicitly casts `default` to `CanBeNull<TIn>`
            TOut result = func(default); // No error: null allowed, 
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference.
        }

        /// <summary>
        /// Mark `TIn` as CanBeNull, `TOut` as CanNotBeNull.
        /// Ensure result is not null.
        /// </summary>
        public void Test5<TIn, TOut>(Func<CanBeNull<TIn>, CanNotBeNull<TOut>> func)
        {
            // Implicitly casts `default` to `CanBeNull<TIn>`
            CanNotBeNull<TOut> result = func(default(TIn)); // No error: null allowed, 
            Console.WriteLine(result.Value.ToString()); // No error: `Value` is not null here
        }

        TOut Test5<TIn, TOut>(Func<CanBeNull<TIn>, CanNotBeNull<TOut>> func, [AllowNull] TIn value)
        {
            // Error CS8604: Possible null reference argument.
            CanNotBeNull<TOut> result = func(new CanBeNull<TIn>(value)); // No error: null allowed, 

            Console.WriteLine(result.Value.ToString()); // No error: `Value` is not null here
            return result;
        }

        [Fact]
        public void Test5_test()
        {
            Test5<string, string>(a => a.Value ?? "null", "test").Should().Be("test");
            Test5<string, string>(a => a.Value ?? "null", null).Should().Be("null");
        }

        /// <summary>
        /// Mark `TIn` as CanBeNull, `TOut` as CanNotBeNull.
        /// Ensure result is not null.
        /// </summary>
        public void Test51<TIn, TOut>(Func<CanBeNull<TIn>, CanNotBeNull<TOut>> func)
        {
            // Implicitly casts `default` to `CanBeNull<TIn>`
            TOut result = func(default); // No error: null allowed, 

            //!!! Error CS8602: although implicit operator return marked as NotNull !!!
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference
        }

        /// <summary> Custom delegate with nullable in and nullable out.</summary>
        [return: MaybeNull]
        public delegate TResult NullableFunc<TSource, TResult>([AllowNull] TSource x);

        /// <summary>
        /// Use custom delegate: NullableFunc
        /// </summary>
        public void Test6<TIn, TOut>(NullableFunc<TIn, TOut> func)
        {
            TOut result = func.Invoke(default); // no error: null allowed
            Console.WriteLine(result.ToString()); // Error CS8602: Dereference of a possibly null reference
        }

        /// <summary> Custom delegate with nullable in and not nullable out.</summary>
        [return: NotNull]
        public delegate TResult NullableInNotNullableOutFunc<TSource, TResult>([AllowNull] TSource x);

        /// <summary>
        /// Use custom delegate: NullableInNotNullableOutFunc
        /// </summary>
        public void Test7<TIn, TOut>(NullableInNotNullableOutFunc<TIn, TOut> func)
        {
            TOut result = func.Invoke(default); // No error: null allowed.
            Console.WriteLine(result.ToString()); // No error: `result` is not null here.
        }

        /*

        You can use two solutions:

        First: Create wrappers `CanBeNull<T>` and `CanNotBeNull<T>` and use it in delegates. See sample: Test2, Test3, Test4, Test5. 
        This approach allows to mix nullable and not nullable value in delegates and do it in explicit manner without big code rewrite.


        Second: Create custom delegates like `NullableFunc` or `NullableInNotNullableOutFunc` see samples: Test6, Test7

        ```
        /// <summary> Custom delegate with nullable in and nullable out.</summary>
        [return: MaybeNull]
        public delegate TResult NullableFunc<TSource, TResult>([AllowNull] TSource x);
        ```

        ```
        /// <summary> Custom delegate with nullable in and not nullable out.</summary>
        [return: NotNull]
        public delegate TResult NullableInNotNullableOutFunc<TSource, TResult>([AllowNull] TSource x);
        ```

        */
    }
}
