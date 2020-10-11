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
        new void Add(TMessage message);

        /// <summary>
        /// Appends other messages and returns new message list.
        /// </summary>
        /// <param name="other">Other messages.</param>
        new void AddRange(IEnumerable<TMessage> other);
    }
}
