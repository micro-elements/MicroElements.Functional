// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Mutable message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public interface IMutableMessageList<TMessage> : IMessageList<TMessage>
    {
        /// <summary>
        /// Adds message to list.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>The same list instance.</returns>
        new IMutableMessageList<TMessage> Add(TMessage message);

        /// <summary>
        /// Appends other messages and returns new message list.
        /// </summary>
        /// <param name="messages">Other messages.</param>
        /// <returns>The same list instance.</returns>
        new IMutableMessageList<TMessage> AddRange(IEnumerable<TMessage> messages);
    }
}
