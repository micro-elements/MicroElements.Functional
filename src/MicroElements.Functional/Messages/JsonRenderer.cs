using System;
using System.IO;

namespace MicroElements.Functional
{
    /// <summary>
    /// Json renderer.
    /// </summary>
    public class JsonRenderer : IMessageRenderer
    {
        /// <inheritdoc />
        public void Render(IMessage message, TextWriter output)
        {
            output
                .WriteJsonObjectStart()
                .WriteText(message.OriginalMessage)
                .WriteJsonObjectEnd()
                .WriteNewLine();
        }
    }

    internal static class JsonFormatterExt
    {
        internal static TextWriter WriteText(this TextWriter output, string text)
        {
            output.Write(text);
            return output;
        }

        internal static TextWriter WriteText(this TextWriter output, char c)
        {
            output.Write(c);
            return output;
        }

        internal static TextWriter WriteJsonObjectStart(this TextWriter output) =>
            output.WriteText('{');

        internal static TextWriter WriteJsonObjectEnd(this TextWriter output) =>
            output.WriteText('}');

        internal static TextWriter WriteNewLine(this TextWriter output) =>
            output.WriteText(Environment.NewLine);

        internal static TextWriter WriteJsonProperty(this TextWriter output, string name, string value) =>
            output.WriteText($"\"{name}\": \"{value}\"");
    }
}
