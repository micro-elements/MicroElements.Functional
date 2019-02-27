namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates <see cref="Message"/> with <see cref="MessageSeverity.Error"/>.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <returns><see cref="Message"/> with <see cref="MessageSeverity.Error"/>.</returns>
        public static Message ErrorMessage(string text) => new Message(text, severity: MessageSeverity.Error);

        /// <summary>
        /// Creates <see cref="Message"/> with <see cref="MessageSeverity.Warning"/>.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <returns><see cref="Message"/> with <see cref="MessageSeverity.Warning"/>.</returns>
        public static Message WarningMessage(string text) => new Message(text, severity: MessageSeverity.Warning);

        /// <summary>
        /// Creates <see cref="Message"/> with <see cref="MessageSeverity.Information"/>.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <returns><see cref="Message"/> with <see cref="MessageSeverity.Information"/>.</returns>
        public static Message InformationMessage(string text) => new Message(text, severity: MessageSeverity.Information);
    }
}
