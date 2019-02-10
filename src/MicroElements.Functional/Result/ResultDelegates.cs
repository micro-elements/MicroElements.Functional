// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    //TODO: see: https://lostechies.com/jimmybogard/2008/03/26/stop-creating-custom-delegate-types/

    /// <summary>
    /// Represents success action.
    /// </summary>
    /// <typeparam name="A">Result type.</typeparam>
    /// <param name="value">Result value.</param>
    public delegate void SuccessAction<in A>(A value);

    /// <summary>
    /// Represents success action.
    /// </summary>
    /// <typeparam name="A">Result type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    /// <param name="value">Result value.</param>
    /// <param name="messages">Result messages.</param>
    public delegate void SuccessAction<in A, Message>(A value, IMessageList<Message> messages);

    /// <summary>
    /// Represents success function.
    /// </summary>
    /// <typeparam name="A">Result type.</typeparam>
    /// <typeparam name="B">Output type.</typeparam>
    /// <param name="value">Result value.</param>
    /// <returns>Output value.</returns>
    public delegate B SuccessFunc<in A, out B>(A value);

    /// <summary>
    /// Represents success function.
    /// </summary>
    /// <typeparam name="A">Result type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    /// <typeparam name="B">Output type.</typeparam>
    /// <param name="value">Result value.</param>
    /// <param name="messages">Result messages.</param>
    /// <returns>Output value.</returns>
    public delegate B SuccessFunc<in A, Message, out B>(A value, IMessageList<Message> messages);

    /// <summary>
    /// Represents error action.
    /// </summary>
    /// <typeparam name="Error">Error type.</typeparam>
    /// <param name="error">Error value.</param>
    public delegate void ErrorAction<in Error>(Error error);

    /// <summary>
    /// Represents error action.
    /// </summary>
    /// <typeparam name="Error">Error type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    /// <param name="error">Error value.</param>
    /// <param name="messages">Messages.</param>
    public delegate void ErrorAction<in Error, Message>(Error error, IMessageList<Message> messages);

    /// <summary>
    /// Represents error function.
    /// </summary>
    /// <typeparam name="Error">Error type.</typeparam>
    /// <typeparam name="B">Output type.</typeparam>
    /// <param name="error">Error value.</param>
    /// <returns>Output value.</returns>
    public delegate B ErrorFunc<in Error, out B>(Error error);

    /// <summary>
    /// Represents error function.
    /// </summary>
    /// <typeparam name="Error">Error type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    /// <typeparam name="B">Output type.</typeparam>
    /// <param name="error">Error value.</param>
    /// <param name="messages">Messages.</param>
    /// <returns>Output value.</returns>
    public delegate B ErrorFunc<in Error, Message, out B>(Error error, IMessageList<Message> messages);

    /// <summary>
    /// ResultDelegateExtensions.
    /// </summary>
    public static class ResultDelegateExtensions
    {
        /// <summary>
        /// Converts <see cref="SuccessAction{A}"/> to <see cref="SuccessFunc{A, Unit}"></see>.
        /// </summary>
        /// <typeparam name="A">A type.</typeparam>
        /// <param name="action">SuccessAction.</param>
        /// <returns>SuccessFunc.</returns>
        public static SuccessFunc<A, Unit> ToFunc<A>(this SuccessAction<A> action)
            => (a) => { action(a); return Unit(); };

        /// <summary>
        /// Converts <see cref="SuccessAction{A, Message}"/> to <see cref="SuccessFunc{A, Message, Unit}"></see>.
        /// </summary>
        /// <typeparam name="A">A type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="action">SuccessAction.</param>
        /// <returns>SuccessFunc.</returns>
        public static SuccessFunc<A, Message, Unit> ToFunc<A, Message>(this SuccessAction<A, Message> action)
            => (a, m) => { action(a, m); return Unit(); };

        /// <summary>
        /// Converts <see cref="ErrorAction{Error}"/> to <see cref="ErrorFunc{Error, Unit}"></see>.
        /// </summary>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <param name="action">ErrorAction.</param>
        /// <returns>ErrorFunc.</returns>
        public static ErrorFunc<Error, Unit> ToFunc<Error>(this ErrorAction<Error> action)
            => (e) => { action(e); return Unit(); };

        /// <summary>
        /// Converts <see cref="ErrorAction{Error, Message}"/> to <see cref="ErrorFunc{Error, Message, Unit}"></see>.
        /// </summary>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="action">ErrorAction.</param>
        /// <returns>ErrorFunc.</returns>
        public static ErrorFunc<Error, Message, Unit> ToFunc<Error, Message>(this ErrorAction<Error, Message> action)
            => (e, m) => { action(e, m); return Unit(); };
    }
}
