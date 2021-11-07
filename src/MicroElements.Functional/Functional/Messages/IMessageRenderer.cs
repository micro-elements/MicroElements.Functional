// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace MicroElements.Functional
{
    /// <summary>
    /// Renders message to text writer.
    /// </summary>
    public interface IMessageRenderer
    {
        /// <summary>
        /// Renders message to text writer.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="output">Target output.</param>
        void Render(IMessage message, TextWriter output);
    }
}
