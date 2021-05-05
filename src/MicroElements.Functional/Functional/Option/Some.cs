// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MicroElements.Reflection;

namespace MicroElements.Functional
{
    /// <summary>
    /// Some wrapper for value. Can not be null.
    /// Implicitly converts to <see cref="Option{A}"/>.
    /// </summary>
    /// <typeparam name="A">Value type.</typeparam>
    public readonly struct Some<A> :
        IOptional
    {
        /// <summary>
        /// The option state.
        /// </summary>
        internal readonly OptionState State;

        /// <summary>
        /// Some value.
        /// </summary>
        private readonly A value;

        /// <summary>
        /// Wrapped NotNull value.
        /// </summary>
        public A Value => IsInitialized ? value : throw new SomeNotInitializedException(typeof(A));

        /// <summary>
        /// Initializes a new instance of the <see cref="Some{A}"/> struct.
        /// </summary>
        /// <param name="value">Value to wrap.</param>
        public Some(A value)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");

            State = OptionState.Some;
            this.value = value;
        }

        /// <summary>
        /// Checks that Some is initialized.
        /// </summary>
        internal bool IsInitialized => State == OptionState.Some;

        /// <inheritdoc />
        public bool IsSome => IsInitialized;

        /// <inheritdoc />
        public bool IsNone => !IsInitialized;

        /// <inheritdoc />
        public Type GetUnderlyingType() => typeof(A);

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, Res> some, Func<Res> none) =>
            IsSome
                ? some(value).AssertNotNullResult()
                : none().AssertNotNullResult();
    }
}
