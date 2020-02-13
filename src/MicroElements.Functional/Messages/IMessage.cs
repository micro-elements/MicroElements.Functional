using System;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents message.
    /// Can be used as simple log message, detailed or structured log message, validation message, diagnostic message.
    /// </summary>
    public interface IMessage : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Date and time of message created.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        MessageSeverity Severity { get; }

        /// <summary>
        /// Original message.
        /// Can be in form of MessageTemplates.org.
        /// </summary>
        string OriginalMessage { get; }

        /// <summary>
        /// Formatted message.
        /// It's a result of MessageTemplate rendered with <seealso cref="Properties"/>.
        /// </summary>
        string FormattedMessage { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Message properties.
        /// </summary>
        IReadOnlyCollection<KeyValuePair<string, object>> Properties { get; }
    }

    public interface ITemplatedMessage
    {
        /// <summary>
        /// Original message.
        /// Can be in form of MessageTemplates.org.
        /// </summary>
        string OriginalMessage { get; }

        /// <summary>
        /// Formatted message.
        /// It's a result of MessageTemplate rendered with <seealso cref="Properties"/>.
        /// </summary>
        string FormattedMessage { get; }
    }

    //public static class MessageExtensions
    //{
    //    public static string Text(this IMessage message)
    //    {
    //        return message.
    //    }
    //}
}
