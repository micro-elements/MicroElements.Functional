namespace MicroElements.Functional
{
    using System;
    using static MicroElements.Functional.Prelude;

    public static class ActionExt
    {
        public static Func<ValueTuple> ToFunc(this Action action)
            => () => { action(); return Unit(); };

        public static Func<T, ValueTuple> ToFunc<T>(this Action<T> action)
            => t => { action(t); return Unit(); };

        public static Func<T1, T2, ValueTuple> ToFunc<T1, T2>(this Action<T1, T2> action)
            => (T1 t1, T2 t2) => { action(t1, t2); return Unit(); };
    }
}
