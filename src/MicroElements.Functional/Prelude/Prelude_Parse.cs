using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;

// ReSharper disable CheckNamespace
namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        [Pure]
        public static Option<T> ChangeType<T>(string text)
        {
            if (text == null)
            {
                return None;
            }

            try
            {
                var val = (T)Convert.ChangeType(text, typeof(T));
                return val;
            }
            catch
            {
                return None;
            }
        }

        [Pure]
        public static Option<long> ParseLong(string value) =>
            Parse<long>(long.TryParse, value);

        [Pure]
        public static Option<int> ParseInt(string value) =>
            Parse<int>(int.TryParse, value);

        [Pure]
        public static Option<int> ParseInt(string value, int fromBase)
        {
            try
            {
                return Convert.ToInt32(value, fromBase);
            }
            catch
            {
                return None;
            }
        }

        [Pure]
        public static Option<short> ParseShort(string value) =>
            Parse<short>(short.TryParse, value);

        [Pure]
        public static Option<char> ParseChar(string value) =>
            Parse<char>(char.TryParse, value);

        [Pure]
        public static Option<sbyte> ParseSByte(string value) =>
            Parse<sbyte>(sbyte.TryParse, value);

        [Pure]
        public static Option<byte> ParseByte(string value) =>
            Parse<byte>(byte.TryParse, value);

        [Pure]
        public static Option<ulong> ParseULong(string value) =>
            Parse<ulong>(ulong.TryParse, value);

        [Pure]
        public static Option<uint> ParseUInt(string value) =>
            Parse<uint>(uint.TryParse, value);

        [Pure]
        public static Option<ushort> ParseUShort(string value) =>
            Parse<ushort>(ushort.TryParse, value);

        [Pure]
        public static Option<float> ParseFloat(string value) =>
            Parse<float>(float.TryParse, value);

        [Pure]
        public static Option<double> ParseDouble(string value) =>
            Parse<double>(double.TryParse, value);

        [Pure]
        public static Option<double> ParseDouble(string value, NumberStyles style, IFormatProvider provider) =>
            ParseNumeric<double>(double.TryParse, value, style, provider);

        [Pure]
        public static Option<decimal> ParseDecimal(string value) =>
            Parse<decimal>(decimal.TryParse, value);

        [Pure]
        public static Option<bool> ParseBool(string value) =>
            Parse<bool>(bool.TryParse, value);

        [Pure]
        public static Option<Guid> ParseGuid(string value) =>
            Parse<Guid>(Guid.TryParse, value);

        [Pure]
        public static Option<DateTime> ParseDateTime(string value) =>
            Parse<DateTime>(DateTime.TryParse, value);

        [Pure]
        public static Option<DateTimeOffset> ParseDateTimeOffset(string value) =>
            Parse<DateTimeOffset>(DateTimeOffset.TryParse, value);

        [Pure]
        public static Option<TEnum> ParseEnum<TEnum>(string value)
            where TEnum : struct =>
            Parse<TEnum>(Enum.TryParse, value);

        [Pure]
        public static Option<IPAddress> ParseIPAddress(string value) =>
                    Parse<IPAddress>(IPAddress.TryParse, value);

        private delegate bool TryParse<T>(string value, out T result);

        private delegate bool TryParseExt<T>(string value, NumberStyles style, IFormatProvider provider, out T result);

        private static Option<T> Parse<T>(TryParse<T> tryParse, string value) =>
            tryParse(value, out T result)
                ? Some(result)
                : None;

        //public static Option<T> TryParse2<T>(Func<T> tryParse, string value) =>
        //    tryParse(value, out T result)
        //        ? Some(result)
        //        : None;

        private static Option<T> ParseNumeric<T>(TryParseExt<T> tryParse, string value, NumberStyles style, IFormatProvider provider) =>
            tryParse(value, style, provider, out T result)
                ? Some(result)
                : None;
    }
}
