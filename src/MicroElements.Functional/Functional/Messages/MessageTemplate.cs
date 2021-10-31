// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MicroElements.CodeContracts;

//using TextSpan = System.ReadOnlySpan<char>; //todo: remove after performance tests (span has no perf win...)
using TextSpan = System.String;

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

        /// <inheritdoc />
        public override string ToString() => $"Format='{OriginalFormat}', Tokens={Tokens.Count}";
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

        /// <summary>
        /// Name slice.
        /// </summary>
        public TextSlice NameSlice { get; }

        /// <summary>
        /// Format applied to the property.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Text alignment. Positive is right aligned, negative is left aligned.
        /// </summary>
        public int Alignment { get; }

        /// <summary>
        /// Creates new instance of <seealso cref="Token"/>.
        /// </summary>
        /// <param name="startIndex">Start index in original message format.</param>
        /// <param name="length">The length of token in symbols.</param>
        /// <param name="tokenType">Token type.</param>
        /// <param name="captureType">CaptureType.</param>
        /// <param name="nameSlice">Text slice for name.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="format">Format applied to the property.</param>
        public Token(
            int startIndex,
            int length,
            TokenType tokenType = TokenType.Text,
            CaptureType captureType = CaptureType.Default,
            TextSlice nameSlice = default,
            int alignment = 0,
            string format = null)
        {
            StartIndex = startIndex;
            Length = length;
            Alignment = alignment;

            TokenType = tokenType;
            CaptureType = captureType;

            NameSlice = nameSlice;
            Alignment = alignment;
            Format = format;
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
    /// WARNING: Implements only basic behavior of MessageTemplates.
    /// </summary>
    public class MessageTemplateParser : IMessageTemplateParser
    {
        /// <summary>
        /// Global static instance.
        /// </summary>
        public static readonly MessageTemplateParser Instance = new MessageTemplateParser();

        private static readonly char[] TextDelimiters = { '{', '}' };
        private static readonly char[] HoleDelimiters = { '}', ':', ',' };
        private static readonly IReadOnlyList<Token> NoTokens = Array.Empty<Token>();

        /// <inheritdoc />
        public MessageTemplate Parse(string messageTemplate)
        {
            var templateSpan = messageTemplate;//.AsSpan();

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
                char nextChar = position < templateSpan.Length - 1 ? templateSpan[position + 1] : char.MinValue;
                if (currentChar == '{' && nextChar != '{')
                {
                    var hole = ParseHole(templateSpan, ref position);
                    tokens.Add(hole);
                }
                else if (currentChar == '}')
                {
                    position++;
                    //todo: error
                }
                else
                {
                    var textToken = ParseText(templateSpan, ref position);
                    tokens.Add(textToken);
                }
            }
            return new MessageTemplate(messageTemplate, tokens);
        }

        private Token ParseText(TextSpan templateSpan, ref int position)
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
            return new Token(startIndex, tokenLength, TokenType.Text);
        }

        private Token ParseHole(TextSpan templateSpan, ref int position)
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

            return new Token(startIndex, length, TokenType.Hole, captureType, name, alignment, format);
        }

        private TextSlice ReadUntil(TextSpan templateSpan, ref int position, char[] symbols)
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
                return new TextSlice(startIndex, stopIndex - startIndex);
            }

            // TODO: Error.
            return default;
        }

        private TextSlice ParseNameOrIndex(TextSpan templateSpan, ref int position)
        {
            return ReadUntil(templateSpan, ref position, HoleDelimiters);
        }

        private int ParseAlignment(TextSpan templateSpan, ref int position)
        {
            int result = 0;
            int sign = 1;
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
                    if (currentChar == '-')
                    {
                        sign = -1;
                        continue;
                    }
                    if (currentChar == ':' || currentChar == '}')
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
                return 0;
            }

            return result * sign;
        }

        private string ParseFormat(TextSpan templateSpan, ref int position)
        {
            string format = null;
            char currentChar = templateSpan[position];
            if (currentChar == ':')
            {
                position++;

                // read digits, skip spaces, until }
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

    /// <summary>
    /// Text slice.
    /// </summary>
    public readonly struct TextSlice
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
        public TextSlice(int startIndex, int length)
        {
            StartIndex = startIndex;
            Length = length;
        }
    }

    /// <summary>
    /// Default implementation of <see cref="IMessageTemplateRenderer"/>.
    /// </summary>
    public class MessageTemplateRenderer : IMessageTemplateRenderer
    {
        /// <summary>
        /// Global static instance.
        /// </summary>
        public static readonly MessageTemplateRenderer Instance = new MessageTemplateRenderer();

        /// <summary>
        /// Renderer provide.
        /// </summary>
        private readonly IValueRendererProvider _valueRendererProvider;

        /// <inheritdoc />
        public MessageTemplateRenderer(IValueRendererProvider valueRendererProvider = null)
        {
            _valueRendererProvider = valueRendererProvider ?? DefaultValueRendererProvider.Instance;
        }

        /// <inheritdoc />
        public void Render(MessageTemplate messageTemplate, IReadOnlyDictionary<string, object> properties, TextWriter output)
        {
            var templateSpan = messageTemplate.OriginalFormat;//.AsSpan();

            foreach (var token in messageTemplate.Tokens)
            {
                if (token.TokenType == TokenType.Text)
                {
                    WriteTextToken(output, templateSpan, token);
                }
                else
                {
                    //var propName = templateSpan.Slice(token.NameSlice.StartIndex, token.NameSlice.Length).ToString();
                    var propName = templateSpan.Substring(token.NameSlice.StartIndex, token.NameSlice.Length);
                    if (!properties.TryGetValue(propName, out object propValue))
                    {
                        WriteTextToken(output, templateSpan, token);
                        continue;
                    }

                    string? textValue = token.Format != null ?
                        RenderValueWithFormat(token, propValue) :
                        RenderToString(propValue);

                    if (textValue != null)
                    {
                        if (token.Alignment != 0)
                        {
                            output.Write($"{{0,{token.Alignment}}}", textValue);
                        }
                        else
                        {
                            output.WriteText(textValue);
                        }
                    }
                }
            }
        }

        private string? RenderValueWithFormat(Token token, object propValue)
        {
            var formats = ParseFormats(token.Format);
            foreach (var format in formats)
            {
                var renderer = _valueRendererProvider.Get(format.Name, format.Args);
                propValue = renderer != null ? renderer.Render(propValue) : RenderToString(propValue, format.Name);
            }

            return RenderToString(propValue);
        }

        private static string? RenderToString(object? propValue, string? format = null)
        {
            if (propValue is null)
                return null;

            if (propValue is string textValue)
                return textValue;

            if (format != null && propValue is IFormattable formattable)
            {
                return formattable.ToString(format, null);
            }

            return propValue.ToString();
        }

        /// <summary>
        /// One of the format part.
        /// </summary>
        private readonly struct Format
        {
            public readonly string Name;
            public readonly string Args;

            public Format(string name, string args)
            {
                Name = name;
                Args = args;
            }
        }

        private IEnumerable<Format> ParseFormats(string format)
        {
            if (string.IsNullOrEmpty(format))
                yield break;

            int argsStart1 = format.IndexOf('(');
            int argsEnd1 = argsStart1 > 0 ? format.IndexOf(')', argsStart1) : -1;
            if (argsStart1 > 0 && argsEnd1 > 0)
            {
                string[] formats = format.Split(new []{':', ' '}, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < formats.Length; i++)
                {
                    var form = formats[i];

                    int argsStart, argsEnd = -1;

                    if (i == 0)
                    {
                        argsStart = argsStart1;
                        argsEnd = argsEnd1;
                    }
                    else
                    {
                        argsStart = form.IndexOf('(');
                        if (argsStart > 0)
                        {
                            argsEnd = form.IndexOf(')', argsStart);
                        }
                    }

                    if (argsStart > 0 && argsEnd > 0)
                    {
                        string formatName = form.Substring(0, argsStart);

                        if (argsEnd - argsStart > 1)
                        {
                            string argsText = form.Substring(argsStart + 1, argsEnd - argsStart - 1);
                            yield return new Format(formatName, argsText);
                        }
                        else
                        {
                            yield return new Format(formatName, null);
                        }
                    }
                }
            }
            else
            {
                yield return new Format(format, null);
            }
        }

        private static void WriteTextToken(TextWriter output, TextSpan templateSpan, in Token token)
        {
            for (int i = token.StartIndex; i < token.StartIndex + token.Length; i++)
            {
                output.Write(templateSpan[i]);
            }
        }
    }

    /// <summary>
    /// ValueRendererProvider provides <see cref="IValueRenderer"/> by name.
    /// </summary>
    public interface IValueRendererProvider
    {
        /// <summary>
        /// Gets <see cref="IValueRenderer"/> by name.
        /// </summary>
        /// <param name="name">Renderer name.</param>
        /// <param name="args">Renderer optional args.</param>
        /// <returns>Renderer or null if not found.</returns>
        IValueRenderer Get(string name, string args);
    }

    /// <summary>
    /// Default providers registers renderers: upper, trim(len).
    /// </summary>
    public class DefaultValueRendererProvider : IValueRendererProvider
    {
        /// <summary>
        /// Default global renderer provider.
        /// </summary>
        public static readonly IValueRendererProvider Instance = new DefaultValueRendererProvider();

        private readonly IValueRendererProvider _rendererProvider = new CachedValueRendererProvider(new Dictionary<string, Type>
        {
            { "upper", typeof(UpperRenderer) },
            { "trim", typeof(TrimRenderer) }
        });

        /// <inheritdoc />
        public IValueRenderer Get(string name, string args) => _rendererProvider.Get(name, args);
    }

    /// <summary>
    /// Cached provider caches renderer for each pair: (name, arg).
    /// </summary>
    public class CachedValueRendererProvider : IValueRendererProvider
    {
        private readonly ConcurrentDictionary<string, Type> _rendererTypes = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<(string name, string args), IValueRenderer> _rendererCache = new ConcurrentDictionary<(string name, string args), IValueRenderer>();

        /// <inheritdoc />
        public CachedValueRendererProvider(IDictionary<string, Type> rendererTypes)
        {
            foreach (var rendererType in rendererTypes)
            {
                _rendererTypes.TryAdd(rendererType.Key, rendererType.Value);
            }
        }

        /// <inheritdoc />
        public IValueRenderer Get(string name, string args)
        {
            if (_rendererCache.TryGetValue((name, args), out IValueRenderer renderer))
            {
                return renderer;
            }

            if (_rendererTypes.TryGetValue(name, out Type rendererType))
            {
                var constructorWithArgs = rendererType.GetConstructor(new Type[] { typeof(string) });
                object[] ctorArgs = constructorWithArgs != null ? new object[] { args } : null;
                renderer = (IValueRenderer)Activator.CreateInstance(rendererType, ctorArgs);
                _rendererCache.TryAdd((name, args), renderer);
            }

            return renderer;
        }
    }

    /// <summary>
    /// Value renderer renders value to string.
    /// Also it can generate new value or transform input value.
    /// </summary>
    public interface IValueRenderer
    {
        /// <summary>
        /// Renders value to string.
        /// Can generate new value or transform input value.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <returns></returns>
        object Render(object? value);
    }

    /// <summary>
    /// Renderer that treats input value as string and renders result as string.
    /// </summary>
    public abstract class StringRenderer : IValueRenderer
    {
        /// <summary>
        /// Renders string value to string result.
        /// </summary>
        /// <param name="textValue">Input value.</param>
        /// <returns>Result value.</returns>
        protected abstract string RenderString(string textValue);

        /// <inheritdoc />
        public object Render(object? value)
        {
            string text = ValueToString(value);
            return RenderString(text);
        }

        private static string ValueToString(object? value)
        {
            if (value == null)
                return string.Empty;

            string text;
            if (value is string str)
                text = str;
            else
                text = value.ToString();
            return text;
        }
    }

    /// <summary>
    /// Renders string value as UPPERCASE string.
    /// </summary>
    public sealed class UpperRenderer : StringRenderer
    {
        /// <inheritdoc />
        protected override string RenderString(string textValue) => textValue.ToUpperInvariant();
    }

    /// <summary>
    /// Trims first N symbols.
    /// </summary>
    public sealed class TrimRenderer : StringRenderer
    {
        private readonly int _length;

        public TrimRenderer(string args)
        {
            if (!int.TryParse(args, out _length))
            {
                _length = -1;
            }
        }

        /// <inheritdoc />
        protected override string RenderString(string textValue)
        {
            if (_length > 0 && _length <= textValue.Length)
            {
                string result = textValue.Substring(0, _length);
                return result;
            }

            return textValue;
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
            var properties = ArgsToDictionary(messageTemplate, args);
            renderer.Render(messageTemplate, properties, output);
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

        public static string TryRenderToString(
            this IMessageTemplateRenderer renderer,
            MessageTemplate messageTemplate,
            IReadOnlyDictionary<string, object> properties,
            string originalMessage)
        {
            try
            {
                if (messageTemplate.Tokens.Count == 0)
                    return originalMessage;

                var stringWriter = new StringWriter();
                renderer.Render(messageTemplate, properties, stringWriter);
                return stringWriter.ToString();
            }
            catch
            {
                return originalMessage;
            }
        }

        public static string RenderToString(
            this IMessageTemplateRenderer renderer,
            MessageTemplate messageTemplate,
            object[] args)
        {
            var stringWriter = new StringWriter();
            var properties = ArgsToDictionary(messageTemplate, args);
            renderer.Render(messageTemplate, properties, stringWriter);
            return stringWriter.ToString();
        }

        /// <summary>
        /// Tries to capture message properties from <see cref="MessageTemplate"/> and args list.
        /// </summary>
        /// <param name="messageTemplate">Parsed <see cref="MessageTemplate"/>.</param>
        /// <param name="args">Template args.</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, object> ArgsToDictionary(this MessageTemplate messageTemplate, params object[] args)
        {
            args.AssertArgumentNotNull(nameof(args));

            Dictionary<string, object> properties = messageTemplate.Tokens
                .Where(token => token.TokenType == TokenType.Hole)
                .Select(token => messageTemplate.OriginalFormat.Substring(token.NameSlice.StartIndex, token.NameSlice.Length))
                .Zip(args, (name, arg) => (name, arg))
                .ToDictionary(tuple => tuple.name, tuple => tuple.arg);
            return properties;
        }
    }
}
