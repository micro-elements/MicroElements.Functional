using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class ErrorTests
    {
        [Serializable]
        public class BaseError
        {
        }

        public class Error1 : BaseError
        {
        }

        [Serializable]
        public class CustomClassError : BaseError, IEquatable<CustomClassError>
        {
            public ICollection<string> ValidationErrors { get; }

            /// <inheritdoc />
            public CustomClassError(ICollection<string> validationErrors)
            {
                validationErrors.AssertArgumentNotNull(nameof(validationErrors));

                ValidationErrors = validationErrors;
            }

            /// <inheritdoc />
            public bool Equals(CustomClassError? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return ValidationErrors.SequenceEqual(other.ValidationErrors);
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((CustomClassError) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return ValidationErrors.GetHashCode();
            }

            public static bool operator ==(CustomClassError? left, CustomClassError? right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(CustomClassError? left, CustomClassError? right)
            {
                return !Equals(left, right);
            }
        }

        public enum TestErrorCode
        {
            Undefined,
            BusinessError,
            DatabaseError
        }

        public class CustomErrorException : ExceptionWithError<int>
        {
            /// <inheritdoc />
            public CustomErrorException(Error<int> error) : base(error)
            {
            }

            /// <inheritdoc />
            protected CustomErrorException()
            {
            }

            /// <inheritdoc />
            protected CustomErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Fact]
        public void error_is_covariant()
        {
            IError<Error1> error1 = Error.CreateError<Error1>(new Error1(), "error1");
            IError<CustomClassError> error2 = Error.CreateError<CustomClassError>(new CustomClassError(new []{"error1, error2"}), "error2");
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

            IError<int>? error3 = Error.Try<int>(() => throw new CustomErrorException(new Error<int>(-1, "error1")));
            error3.Should().NotBeNull();
            error3.ErrorCode.Should().Be(-1);
            error3.Message.FormattedMessage.Should().Be("error1");
        }

        [Fact]
        public void error_should_be_serializable()
        {
            Error.CreateError<string>("TestErrorCode", "TestError").Should().BeBinarySerializable();

            Error.CreateError<TestErrorCode>(TestErrorCode.DatabaseError, "DatabaseError").Should().BeBinarySerializable();

            Error.CreateError<CustomClassError>(new CustomClassError(new[] { "error1, error2" }), "TestError")
                .Should().BeBinarySerializable();

            Action createNullable = () => new ExceptionWithError<CustomClassError>(error: null);
            createNullable.Should().Throw<ArgumentNullException>();

            new ExceptionWithError<TestErrorCode>(Error.CreateError<TestErrorCode>(TestErrorCode.DatabaseError, "DatabaseError"))
                .Should()
                .BeBinarySerializable()
                .And.BeXmlSerializable();
        }

        [Fact]
        public void message_should_be_serializable()
        {
            Fixture fixture = new Fixture();
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IMessageTemplateParser),
                    typeof(MessageTemplateParser)));
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IMessageTemplateRenderer),
                    typeof(MessageTemplateRenderer)));
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IValueRendererProvider),
                    typeof(DefaultValueRendererProvider)));

            object message = fixture.Create<Message>();

            message.Should().BeBinarySerializable();

            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            json.Should().NotBeEmpty();
            Message deserialized = JsonConvert.DeserializeObject<Message>(json);
        }

    }
}
