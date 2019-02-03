using System;

namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        public static Result<A, Exception, Message> SuccessResult<A, Message>(A value, Message message)
        {
            return Result.Success<A, Exception, Message>(value, new MessageList<Message>(message));
        }

        public static SuccessResult<A> SuccessResult<A>(A value)
        {
            return new SuccessResult<A>(value);
        }
    }
}
