using System;
using System.Runtime.Serialization;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class ErrorTests
    {
        public class BaseError
        {
        }

        public class Error1 : BaseError
        {
        }

        public class Error2 : BaseError
        {
        }

        public class ErrorException : ExceptionWithError<int>
        {
            /// <inheritdoc />
            public ErrorException(Error<int> error) : base(error)
            {
            }

            /// <inheritdoc />
            protected ErrorException()
            {
            }

            /// <inheritdoc />
            protected ErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Fact]
        public void error_is_covariant()
        {
            IError<Error1> error1 = Error.CreateError<Error1>(new Error1(), "error1");
            IError<Error2> error2 = Error.CreateError<Error2>(new Error2(), "error2");
            IError<BaseError> errorBase = error1; // covariant
            errorBase.Should().BeSameAs(error1);
        }

        [Fact]
        public void error_nullability()
        {
            Action createWithNull = () => Error.CreateError<Error1>(null, "error1");
            createWithNull.Should().Throw<ArgumentNullException>();

            Error<string> error = Error.CreateError<string>("errorCode", "error");
            error.Should().NotBeNull();
            error.ErrorCode.Should().NotBeNull();
            error.Message.Should().NotBeNull();

            // warning
            Action createNullable = () => Error.CreateError<string?>("errorCode", "error");
            createNullable.Should().NotThrow();

            createNullable = () => Error.CreateError<int?>(null, "error");
            createNullable.Should().Throw<ArgumentNullException>();

            Func<IError<BaseError>> createNonNull = () => Error.CreateError<Error1>(new Error1(), "error1");
            createNonNull.Should().NotThrow();

            Error.Try<BaseError>(()=>null, new BaseError())
                .Should().BeNull();

            Error.Try<int>(() => null, -1)
                .Should().BeNull();

            Error.Try<int>(() => throw new InvalidOperationException(), -1)
                .Should().Be(Error.CreateError<int>(-1, "Operation is not valid due to the current state of the object."));

            Error.Try<int>(() => throw new InvalidOperationException())
                .Should().Be(Error.CreateError<int>(0, "Operation is not valid due to the current state of the object."));

            IError<int>? error3 = Error.Try<int>(() => throw new ErrorException(new Error<int>(-1, "error1")));
            error3.Should().NotBeNull();
            error3.ErrorCode.Should().Be(-1);
            error3.Message.FormattedMessage.Should().Be("error1");
        }
    }
}
