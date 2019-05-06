// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MicroElements.Functional
{
    /// <summary>
    /// A language-neutral specification for 1) capturing, and 2) rendering, structured log events in a format that’s both human-friendly and machine-readable.
    /// </summary>
    /// <seealso cref="https://messagetemplates.org/"/>
    public class MessageTemplate
    {
        /// <summary>
        /// Original format message.
        /// </summary>
        public string OriginalFormat { get; }

        /// <summary>
        /// Tokens for parsed <seealso cref="OriginalFormat"/>.
        /// </summary>
        public IReadOnlyList<Token> Tokens { get; }

        /// <summary>
        /// Crates new instance of <seealso cref="MessageTemplate"/>.
        /// </summary>
        /// <param name="originalFormat">Original format message.</param>
        /// <param name="tokens">Tokens for parsed <seealso cref="OriginalFormat"/>.</param>
        public MessageTemplate(string originalFormat, IReadOnlyList<Token> tokens)
        {
            OriginalFormat = originalFormat ?? "[null]";
            Tokens = tokens ?? Array.Empty<Token>();
        }
    }

    /// <summary>
    /// MessageTemplate parser.
    /// </summary>
    public interface IMessageTemplateParser
    {
        /// <summary>
        /// Parses string message and returns <seealso cref="MessageTemplate"/>.
        /// </summary>
        /// <param name="messageTemplate">Message template string.</param>
        /// <returns>Parsed <seealso cref="MessageTemplate"/>.</returns>
        MessageTemplate Parse(string messageTemplate);
    }

    /// <summary>
    /// Renders <seealso cref="MessageTemplate"/> to text writer.
    /// </summary>
    public interface IMessageTemplateRenderer
    {
        /// <summary>
        /// Renders <seealso cref="MessageTemplate"/> to text writer.
        /// </summary>
        /// <param name="messageTemplate">MessageTemplate.</param>
        /// <param name="properties">Named properties.</param>
        /// <param name="output">Output writer.</param>
        void Render(MessageTemplate messageTemplate, IReadOnlyDictionary<string, object> properties, TextWriter output);
    }

    /// <summary>
    /// MessageTemplate token.
    /// </summary>
    public readonly struct Token
    {
        /// <summary>
        /// Start index in original message format.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// The length of token in symbols.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Token type: Text | Hole.
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// CaptureType: Default | Stringify | Destructure.
        /// </summary>
        public CaptureType CaptureType { get; }

        // public string Name { get; }

        /// <summary>
        /// Format applied to the property.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Creates new instance of <seealso cref="Token"/>.
        /// </summary>
        /// <param name="startIndex">Start index in original message format.</param>
        /// <param name="length">The length of token in symbols.</param>
        /// <param name="tokenType">Token type.</param>
        /// <param name="captureType">CaptureType.</param>
        /// <param name="format">Format applied to the property.</param>
        public Token(
            int startIndex,
            int length,
            TokenType tokenType = TokenType.Text,
            CaptureType captureType = CaptureType.Default,
            string format = null)
        {
            StartIndex = startIndex;
            Length = length;
            Format = format;
            TokenType = tokenType;
            CaptureType = captureType;
        }
    }

    /// <summary>
    /// Token type.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Text.
        /// </summary>
        Text,

        /// <summary>
        /// Hole.
        /// </summary>
        Hole,
    }

    /// <summary>
    /// Instructs the logger on how to store information about provided parameters.
    /// </summary>
    public enum CaptureType
    {
        /// <summary>
        /// Convert known types and objects to scalars, arrays to sequences.
        /// </summary>
        Default,

        /// <summary>
        /// Convert all types to scalar strings. Prefix name with '$'.
        /// </summary>
        Stringify,

        /// <summary>
        /// Convert known types to scalars, destructure objects and collections
        /// into sequences and structures. Prefix name with '@'.
        /// </summary>
        Destructure
    }

    /// <summary>
    /// Default implementation of <seealso cref="IMessageTemplateParser"/>.
    /// Now implements only basic behavior of MessageTemplates.
    /// </summary>
    public class MessageTemplateParser : IMessageTemplateParser
    {
        /// <summary>
        /// Global static instance.
        /// </summary>
        public static readonly MessageTemplateParser Instance = new MessageTemplateParser();

        /// <summary>
        /// Text slice.
        /// </summary>
        public readonly struct Slice
        {
            /// <summary>
            /// Start index in text.
            /// </summary>
            public readonly int StartIndex;

            /// <summary>
            /// The length of slice.
            /// </summary>
            public readonly int Length;

            /// <summary>
            /// Creates new slice.
            /// </summary>
            /// <param name="startIndex">Start index.</param>
            /// <param name="length">Length.</param>
            public Slice(int startIndex, int length)
            {
                StartIndex = startIndex;
                Length = length;
            }
        }

        private static readonly char[] TextDelimiters = { '{', '}' };
        private static readonly char[] HoleDelimiters = { '}', ':', ',' };
        private static readonly IReadOnlyList<Token> NoTokens = Array.Empty<Token>();

        /// <inheritdoc />
        public MessageTemplate Parse(string messageTemplate)
        {
            ReadOnlySpan<char> templateSpan = messageTemplate.AsSpan();

            int expectedHoles = 0;
            for (int i = 0; i < templateSpan.Length; i++)
            {
                if (templateSpan[i] == '{')
                {
                    expectedHoles++;
                }
            }

            if (expectedHoles == 0)
            {
                return new MessageTemplate(messageTemplate, NoTokens);
            }

            var expectedTokens = (expectedHoles * 2) + 1;

            var tokens = new List<Token>(expectedTokens);

            int position = 0;
            while (position < templateSpan.Length)
            {
                char currentChar = templateSpan[position];
                char nextChar = templateSpan[position + 1];
                if (currentChar == '{' && nextChar != '{')
                {
                    var hole = ParseHole(templateSpan, ref position);
                    tokens.Add(hole);
                }
                else if (currentChar == '}')
                {
                    position++;
                    //error
                }
                else
                {
                    var textToken = ParseText(templateSpan, ref position);
                    tokens.Add(textToken);
                }
            }
            return new MessageTemplate(messageTemplate, tokens);
        }

        private Token ParseText(in ReadOnlySpan<char> templateSpan, ref int position)
        {
            int startIndex = position;
            int tokenLength = 0;
            while (position < templateSpan.Length)
            {
                char currentChar = templateSpan[position];
                if (currentChar != '{' && currentChar != '}')
                {
                    tokenLength++;
                    position++;
                }
                else
                {
                    break;
                }
            }
            return new Token(startIndex, tokenLength, TokenType.Text, CaptureType.Default);
        }

        private Token ParseHole(in ReadOnlySpan<char> templateSpan, ref int position)
        {
            char currentChar = templateSpan[position];
            int startIndex = position;

            if (currentChar == '{')
            {
                position++;
            }

            CaptureType captureType;
            switch (currentChar)
            {
                case '@':
                    captureType = CaptureType.Destructure;
                    position++;
                    break;
                case '$':
                    captureType = CaptureType.Stringify;
                    position++;
                    break;
                default:
                    captureType = CaptureType.Default;
                    break;
            }

            var name = ParseNameOrIndex(templateSpan, ref position);
            var alignment = ParseAlignment(templateSpan, ref position);
            var format = ParseFormat(templateSpan, ref position);
            currentChar = templateSpan[position];
            if (currentChar == '}')
            {
                position++;
            }

            int length = position - startIndex;

            return new Token(startIndex, length, TokenType.Hole, captureType, format);
        }

        private ReadOnlySpan<char> ReadUntil(in ReadOnlySpan<char> templateSpan, ref int position, char[] symbols)
        {
            int startIndex = position;
            int stopIndex = 0;
            for (; position < templateSpan.Length; position++)
            {
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i] == templateSpan[position])
                    {
                        stopIndex = position;
                        break;
                    }
                }
                if (stopIndex > 0)
                    break;
            }

            if (stopIndex > 0)
            {
                return templateSpan.Slice(startIndex, stopIndex - startIndex);
            }

            // TODO: Error.
            return default;
        }

        private void ReadUntil(in ReadOnlySpan<char> templateSpan, ref int position, char symbol, Action<char> onChar)
        {
            for (; position < templateSpan.Length; position++)
            {
                char c = templateSpan[position];
                if (c == ' ')
                    continue;
                if (c == symbol)
                    break;
                onChar(c);
            }
        }

        private void ReadUntil(
            in ReadOnlySpan<char> templateSpan,
            ref int position,
            Func<char, bool> until,
            Func<char, bool> verify,
            Action<char> onChar)
        {
            for (; position < templateSpan.Length; position++)
            {
                char c = templateSpan[position];
                if (c == ' ')
                    continue;
                if (until(c))
                    break;
                if (verify(c))
                    onChar(c);
            }
        }

        private ReadOnlySpan<char> ParseNameOrIndex(in ReadOnlySpan<char> templateSpan, ref int position)
        {
            return ReadUntil(templateSpan, ref position, HoleDelimiters);
        }

        private Option<int> ParseAlignment(in ReadOnlySpan<char> templateSpan, ref int position)
        {
            int result = 0;
            char currentChar = templateSpan[position];
            if (currentChar == ',')
            {
                position++;
                //read digits, skip spaces, until }
                for (; position < templateSpan.Length; position++)
                {
                    currentChar = templateSpan[position];
                    if (currentChar == ' ')
                        continue;
                    if (currentChar == '}')
                        break;
                    if (currentChar >= '0' && currentChar <= '9')
                    {
                        int digit = currentChar - '0';
                        result = result * 10 + digit;
                    }
                    else
                    {
                        // TODO: Error.
                    }
                }
            }
            else
            {
                return Option<int>.None;
            }

            return result;
        }

        private string ParseFormat(in ReadOnlySpan<char> templateSpan, ref int position)
        {
            string format = null;
            char currentChar = templateSpan[position];
            if (currentChar == ':')
            {
                position++;
                //read digits, skip spaces, until }
                for (; position < templateSpan.Length; position++)
                {
                    currentChar = templateSpan[position];
                    if (currentChar == ' ')
                        continue;
                    if (currentChar == '}')
                        break;
                    format += currentChar;
                }
            }

            return format;
        }
    }

    public class MessageTemplateRenderer : IMessageTemplateRenderer
    {
        /// <summary>
        /// Global static instance.
        /// </summary>
        public static readonly MessageTemplateRenderer Instance = new MessageTemplateRenderer();

        /// <inheritdoc />
        public void Render(MessageTemplate messageTemplate, IReadOnlyDictionary<string, object> properties, TextWriter output)
        {
            ReadOnlySpan<char> templateSpan = messageTemplate.OriginalFormat.AsSpan();

            foreach (var token in messageTemplate.Tokens)
            {
                if (token.TokenType == TokenType.Text)
                {
                    WriteTextToken(output, templateSpan, token);
                }
                else
                {
                    var slice = templateSpan.Slice(token.StartIndex, token.Length);
                    var name = slice.Slice(1, slice.Length - 2);
                    string propName = name.ToString();//TODO: token.Name or get by token name index and len
                    object propValue;
                    if (!properties.TryGetValue(propName, out propValue))
                    {
                        WriteTextToken(output, templateSpan, token);
                        continue;
                    }

                    if (token.Format != null)
                        output.Write($"{0}:{token.Format}", propValue);
                    else
                        output.Write("{0}", propValue);
                }
            }
        }

        private static void WriteTextToken(TextWriter output, ReadOnlySpan<char> templateSpan, Token token)
        {
            var slice = templateSpan.Slice(token.StartIndex, token.Length);
            for (int i = 0; i < slice.Length; i++)
            {
                output.Write(slice[i]);
            }
        }
    }

    public static class MessageTemplateParserExtensions
    {
        public static MessageTemplate TryParse(this IMessageTemplateParser messageTemplateParser, string messageTemplate)
        {
            try
            {
                return messageTemplateParser.Parse(messageTemplate);
            }
            catch (Exception e)
            {
                return new MessageTemplate(messageTemplate, null);
            }
        }
    }

    public static class MessageTemplateRendererExtensions
    {
        public static void Render(
            this IMessageTemplateRenderer renderer,
            MessageTemplate messageTemplate,
            object[] args,
            TextWriter output)
        {
            Dictionary<string, object> properties = ArgsToDictionary(args);
            renderer.Render(messageTemplate, properties, output);
        }

        private static Dictionary<string, object> ArgsToDictionary(object[] args)
        {
            var properties = args
                .Select((arg, index) => (arg, index))
                .ToDictionary(tuple => tuple.index.ToString(), tuple => tuple.arg);
            return properties;
        }

        public static string RenderToString(
            this IMessageTemplateRenderer renderer,
            MessageTemplate messageTemplate,
            IReadOnlyDictionary<string, object> properties)
        {
            var stringWriter = new StringWriter();
            renderer.Render(messageTemplate, properties, stringWriter);
            return stringWriter.ToString();
        }

        public static string RenderToString(
            this IMessageTemplateRenderer renderer,
            MessageTemplate messageTemplate,
            object[] args)
        {
            var stringWriter = new StringWriter();

            Dictionary<string, object> properties = messageTemplate.Tokens.Where(token => token.TokenType == TokenType.Hole)
                .Select(token => messageTemplate.OriginalFormat.Substring(token.StartIndex, token.Length))
                .Zip(args, (s, o) => (s, o))
                .ToDictionary(tuple => tuple.s, tuple => tuple.o);

            renderer.Render(messageTemplate, properties, stringWriter);
            return stringWriter.ToString();
        }
    }

    public class SafeMessageTemplateParser : IMessageTemplateParser
    {
        private readonly IMessageTemplateParser _messageTemplateParser;

        public SafeMessageTemplateParser(IMessageTemplateParser messageTemplateParser)
        {
            _messageTemplateParser = messageTemplateParser;
        }

        /// <inheritdoc />
        public MessageTemplate Parse(string messageTemplate)
        {
            try
            {
                return _messageTemplateParser.Parse(messageTemplate);
            }
            catch (Exception e)
            {
                return new MessageTemplate(messageTemplate, null);
            }
        }
    }
}
