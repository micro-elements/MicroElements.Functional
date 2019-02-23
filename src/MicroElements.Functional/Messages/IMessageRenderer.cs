using System;
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
