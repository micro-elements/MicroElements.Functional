using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    public struct MOption<A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> some, Func<B> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return opt.IsSome
                ? Check.NotNullResult(some(opt.Value))
                : Check.NotNullResult(none());
        }

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> some, B none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            return opt.IsSome
                ? Check.NotNullResult(some(opt.Value))
                : none;
        }

        [Pure]
        public Unit Match(Option<A> opt, Action<A> some, Action none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (opt.IsSome)
                some(opt.Value);
            else
                none();
            return Unit.Default;
        }
    }
}
