// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Discriminated union type. Can be in one of two states: Some(a) or None
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public struct Option<T>
    {
        private bool isSome;
        private T value;

        /// <summary>
        /// None.
        /// </summary>
        public static readonly Option<T> None = new Option<T>(false, default(T));

        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        [Pure]
        public bool IsSome => isSome;

        /// <summary>
        /// Is the option in a None state.
        /// </summary>
        [Pure]
        public bool IsNone => !IsSome;

        /// <summary>
        /// Gets option Value.
        /// </summary>
        internal T Value
        {
            get
            {
                if (isSome)
                    return value;
                throw new ValueIsNoneException();
            }
        }

        private Option(bool isSome, T value)
        {
            this.isSome = isSome;
            this.value = value;
        }

        public static implicit operator Option<T>(OptionNone none) => MOption<T>.Inst.None;
        public static implicit operator Option<T>(Some<T> some) => new Option<T>(true, some.Value);
        public static implicit operator Option<T>(T value) => value.IsNull() ? None : Prelude.Some(value);

        /// <summary>
        /// Match the two states of the Option and return a non-null Result.
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <param name="Some"></param>
        /// <param name="None"></param>
        /// <returns></returns>
        public Result Match<Result>(Func<T, Result> Some, Func<Result> None)
            => IsSome ? Some(Value) : None();

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
                yield return Value;
        }

        public bool Equals(Option<T> other)
        {
            if (IsNone && other.IsNone)
                return true;

            if (IsSome && other.IsSome)
                return EqualityComparer<T>.Default.Equals(Value, other.Value);

            return false;
        }

        public bool Equals(None<T> none) => IsNone;

        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        public override string ToString() => IsSome ? $"Some({Value})" : "None";
    }

    public struct None<T>
    {
    }

    public struct Some<T>
    {
        internal T Value { get; }

        internal Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }

    public struct MOption<A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public Option<A> None => Option<A>.None;

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null)
                throw new ArgumentNullException(nameof(Some));
            if (None == null)
                throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None());
        }

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, B None)
        {
            if (Some == null)
                throw new ArgumentNullException(nameof(Some));
            return opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None);
        }
    }

    internal static class Check
    {
        internal static T NullReturn<T>(T value) =>
            value.IsNull()
                ? throw new ResultIsNullException()
                : value;
    }

    /// <summary>
    /// Result is null
    /// </summary>
    [Serializable]
    public class ResultIsNullException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException()
            : base("Result is null.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Value is none
    /// </summary>
    [Serializable]
    public class ValueIsNoneException : Exception
    {
        public static readonly ValueIsNoneException Default = new ValueIsNoneException();

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public interface IOptional
    {
        bool IsSome { get; }

        bool IsNone { get; }

        Type GetUnderlyingType();
    }
}
