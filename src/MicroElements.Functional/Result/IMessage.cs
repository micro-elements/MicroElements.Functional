using System;

namespace MicroElements.Functional.Result
{
    /// <summary>
    /// Represents some diagnostic message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Date and time of message created.
        /// </summary>
        DateTimeOffset CreatedTime { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        Severity Severity { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets a value indicating whether the message is error.
        /// </summary>
        bool IsError { get; }
    }
}