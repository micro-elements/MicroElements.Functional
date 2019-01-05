using System;

namespace MicroElements.Functional
{
    public static class OptionExtensions
    {
        public static ValueTuple Match<T>(this Option<T> @this, Action<T> Some, Action None)
            => @this.Match(Some.ToFunc(), None.ToFunc());

        internal static bool IsSome<T>(this Option<T> @this)
            => @this.Match((_) => true, () => false);

        internal static T ValueUnsafe<T>(this Option<T> @this)
            => @this.Match((t) => t, () => { throw new InvalidOperationException(); });

        public static T GetOrElse<T>(this Option<T> opt, T defaultValue)
            => opt.Match((t) => t, () => defaultValue);

        public static T GetOrElse<T>(this Option<T> opt, Func<T> fallback)
            => opt.Match((t) => t, () => fallback());

        public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
            => left.Match((_) => left, () => right);

        public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
            => left.Match((_) => left, () => right());
    }
}
