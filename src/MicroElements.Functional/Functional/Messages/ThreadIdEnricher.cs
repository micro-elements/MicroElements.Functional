// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;

namespace MicroElements.Functional
{
    /// <summary>
    /// Adds "ThreadId" property.
    /// </summary>
    public class ThreadIdEnricher : IMessageEnricher
    {
        /// <inheritdoc />
        public IMessage Enrich(IMessage message) =>
            message.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId);
    }
}
