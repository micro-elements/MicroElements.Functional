// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
