using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    public struct MOption<A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));

            return opt.IsSome
                ? Check.NotNull(Some(opt.Value))
                : Check.NotNull(None());
        }

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, B None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));

            return opt.IsSome
                ? Check.NotNull(Some(opt.Value))
                : None;
        }

        [Pure]
        public Unit Match(Option<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));

            if (opt.IsSome)
                Some(opt.Value);
            else
                None();
            return Unit.Default;
        }
    }
}
