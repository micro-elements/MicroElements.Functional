namespace MicroElements.Functional
{
    /// <summary>
    /// Message enricher enriches message.
    /// It can add additional properties, change message attributes.
    /// </summary>
    public interface IMessageEnricher
    {
        /// <summary>
        /// Enriches message and returns new copy of message with modifications.
        /// </summary>
        /// <param name="message">Message to enrich.</param>
        /// <returns>New copy of message with modifications.</returns>
        IMessage Enrich(IMessage message);
    }
}
