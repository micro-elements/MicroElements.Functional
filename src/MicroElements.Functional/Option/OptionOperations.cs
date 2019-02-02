using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Option operations.
    /// </summary>
    public static partial class OptionOperations
    {
        [Pure]
        public static B Match<A, B>(
            ref Option<A> opt,
            Func<A, B> some,
            Func<B> none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            var res = opt.IsSome
                ? some(opt.Value)
                : none();
            return res.AssertNotNullResult();
        }

        [Pure]
        public static B Match<A, B>(
            ref Option<A> opt,
            Func<A, B> some,
            B none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            var res = opt.IsSome
                ? some(opt.Value)
                : none;
            return res.AssertNotNullResult();
        }

        [Pure]
        public static Unit Match<A>(
            ref Option<A> opt,
            Action<A> some,
            Action none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            if (opt.IsSome)
                some(opt.Value);
            else
                none();
            return Unit.Default;
        }
    }
}
