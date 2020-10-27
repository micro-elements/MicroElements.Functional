﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// ReadOnly message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public interface IMessageList<TMessage> : IReadOnlyCollection<TMessage>
    {
        /// <summary>
        /// Adds message to list.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>New instance of message list with added message.</returns>
        IMessageList<TMessage> Add(TMessage message);

        /// <summary>
        /// Appends other messages and returns new message list.
        /// </summary>
        /// <param name="other">Other messages.</param>
        /// <returns>New list that contains current messages and other messages.</returns>
        IMessageList<TMessage> AddRange(IEnumerable<TMessage> other);
    }
}
