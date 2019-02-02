// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Discriminated union type. Can be in one of two states: Some(a) or None.
    /// </summary>
    /// <typeparam name="A">Value type.</typeparam>
    public struct Option<A> :
        IEnumerable<A>,
        IOptional,
        IEquatable<Option<A>>
    {
        /// <summary>
        /// None.
        /// </summary>
        public static readonly Option<A> None = new Option<A>(false, default(A));

        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        private readonly OptionState _optionState;

        /// <summary>
        /// Option value.
        /// </summary>
        private readonly A _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option{A}"/> struct.
        /// </summary>
        /// <param name="isSome">Is the option in a Some state.</param>
        /// <param name="value">Option value.</param>
        private Option(bool isSome, A value)
        {
            _optionState = isSome ? OptionState.Some : OptionState.None;
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option{A}"/> struct.
        /// </summary>
        /// <param name="option">IEnumerable as value source.</param>
        public Option(IEnumerable<A> option)
        {
            var first = option.Take(1).ToArray();
            if (first.Length == 0)
            {
                _optionState = OptionState.None;
                _value = default(A);
            }
            else
            {
                _optionState = OptionState.Some;
                _value = first[0];
            }
        }

        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        [Pure]
        public bool IsSome => _optionState == OptionState.Some;

        /// <summary>
        /// Is the option in a None state.
        /// </summary>
        [Pure]
        public bool IsNone => _optionState == OptionState.None;

        /// <summary>
        /// Returns option underlying type.
        /// </summary>
        /// <returns>Option underlying type.</returns>
        [Pure]
        public Type GetUnderlyingType() => typeof(A);

        /// <inheritdoc />
        public R MatchUntyped<R>(Func<object, R> some, Func<R> none) =>
            IsSome
                ? some(Value).AssertNotNullResult()
                : none().AssertNotNullResult();

        /// <summary>
        /// Gets the Option Value.
        /// </summary>
        internal A Value
        {
            get
            {
                if (IsSome)
                    return _value;
                throw new ValueIsNoneException();
            }
        }

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items.
        /// </summary>
        /// <returns>An enumerable of zero or one items.</returns>
        public IEnumerable<A> AsEnumerable()
        {
            if (IsSome)
                yield return Value;
        }

        /// <summary>
        /// Gets enumerator for option.
        /// </summary>
        /// <returns>Enumerator for option.</returns>
        public IEnumerator<A> GetEnumerator()
            => AsEnumerable().GetEnumerator();

        /// <summary>
        /// Gets enumerator for option.
        /// </summary>
        /// <returns>Enumerator for option.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => AsEnumerable().GetEnumerator();

        /// <inheritdoc />
        public bool Equals(Option<A> other)
        {
            if (IsNone && other.IsNone)
                return true;

            if (IsSome && other.IsSome)
                return EqualityComparer<A>.Default.Equals(Value, other.Value);

            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Option<A> option)
                return Equals(option);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            IsSome
                ? Value.GetHashCode()
                : 0;

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(Option<A> @this, Option<A> other) => @this.Equals(other);

        /// <summary>
        /// Non-equality operator.
        /// </summary>
        public static bool operator !=(Option<A> @this, Option<A> other) => !(@this == other);

        /// <inheritdoc />
        public override string ToString() => IsSome ? $"Some({Value})" : "None";

        /// <summary>
        /// Implicit conversion of None to Option{T}.
        /// </summary>
        /// <param name="none">None.</param>
        public static implicit operator Option<A>(OptionNone none) => None;

        /// <summary>
        /// Implicit conversion of Some to Option.
        /// </summary>
        /// <param name="some">Some.</param>
        public static implicit operator Option<A>(Some<A> some) => new Option<A>(true, some.Value);

        /// <summary>
        /// Implicit conversion of value to Option.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Option<A>(A value) => value.IsNull() ? None : Prelude.Some(value);

        /// <summary>
        /// Explicit conversion to underlying type.
        /// </summary>
        /// <param name="option">Optional.</param>
        [Pure]
        public static explicit operator A(Option<A> option) =>
            option.IsSome
                ? option.Value
                : throw new InvalidCastException("Option is not in a Some state");

        /// <summary>
        /// Match the two states of the Option and return a non-null Result.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="some">Some match operation.</param>
        /// <param name="none">None match operation.</param>
        /// <returns>Not null Result.</returns>
        public TResult Match<TResult>(Func<A, TResult> some, Func<TResult> none)
            => OptionOperations.Match(ref this, some, none);

        /// <summary>
        /// Match the two states of the Option and return a non-null Result.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="some">Some match operation.</param>
        /// <param name="none">None match operation.</param>
        /// <returns>Not null Result.</returns>
        public TResult Match<TResult>(Func<A, TResult> some, TResult none)
            => OptionOperations.Match(ref this, some, none);

        /// <summary>
        /// Match the two states of the Option.
        /// </summary>
        /// <param name="some">Some match operation.</param>
        /// <param name="none">None match operation.</param>
        /// <returns>Unit.</returns>
        public Unit Match(Action<A> some, Action none)
            => OptionOperations.Match(ref this, some, none);
    }
}
