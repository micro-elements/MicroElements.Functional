namespace MicroElements.Functional
{
    internal static class ToStringBuilder
    {
        internal static string AddIfNotNull(this string text) => text != null ? $"{text} |" : string.Empty;
    }
}