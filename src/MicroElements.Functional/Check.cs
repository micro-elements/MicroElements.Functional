namespace MicroElements.Functional
{
    internal static class Check
    {
        internal static T NotNull<T>(this T value) =>
            value.IsNull()
                ? throw new ResultIsNullException()
                : value;
    }
}
