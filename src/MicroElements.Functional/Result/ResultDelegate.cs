// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    public delegate void SuccessAction<in A>(A value);
    public delegate void SuccessAction<in A, Message>(A value, IMessageList<Message> messages);
    public delegate void ErrorAction<in Error>(Error error);
    public delegate void ErrorAction<in Error, Message>(Error error, IMessageList<Message> messages);
    public delegate B SuccessFunc<in A, out B>(A value);
    public delegate B SuccessFunc<in A, Message, out B>(A value, IMessageList<Message> messages);
    public delegate B ErrorFunc<in Error, out B>(Error error);
    public delegate B ErrorFunc<in Error, Message, out B>(Error error, IMessageList<Message> messages);

    /// <summary>
    /// ActionExtensions.
    /// </summary>
    public static class ResultDelegateExt
    {
        public static SuccessFunc<A, Unit> ToFunc<A>(this SuccessAction<A> action)
            => (a) => { action(a); return Unit(); };

        public static SuccessFunc<A, Message, Unit> ToFunc<A, Message>(this SuccessAction<A, Message> action)
            => (a, m) => { action(a, m); return Unit(); };

        public static ErrorFunc<Error, Unit> ToFunc<Error>(this ErrorAction<Error> action)
            => (e) => { action(e); return Unit(); };

        public static ErrorFunc<Error, Message, Unit> ToFunc<Error, Message>(this ErrorAction<Error, Message> action)
            => (e, m) => { action(e, m); return Unit(); };
    }
}
