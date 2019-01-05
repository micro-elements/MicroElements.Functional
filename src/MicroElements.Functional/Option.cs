using System.Threading.Tasks;

namespace MicroElements.Functional
{
    using System;
    using System.Collections.Generic;
    using static Prelude;

    public struct Option<T>
    {
        private readonly bool isSome;
        private readonly T value;
        private bool isNone => !isSome;

        private Option(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            isSome = true;
            this.value = value;
        }

        public static implicit operator Option<T>(None<T> none) => default(Option<T>);
        public static implicit operator Option<T>(Some<T> some) => new Option<T>(some.Value);
        public static implicit operator Option<T>(T value) => value == null ? None<T>() : Some(value);

        public R Match<R>(Func<T, R> some, Func<R> none)
            => isSome ? some(value) : none();

        public IEnumerable<T> AsEnumerable()
        {
            if (isSome)
                yield return value;
        }

        public bool Equals(Option<T> other)
        {
            if (isNone && other.isNone)
                return true;

            if (isSome && other.isSome)
                return EqualityComparer<T>.Default.Equals(value, other.value);

            return false;
        }

        public bool Equals(None<T> none) => isNone;

        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        public override string ToString() => isSome ? $"Some({value})" : "None";
    }

    public struct None<T>
    {
    }

    public struct Some<T>
    {
        internal T Value { get; }

        internal Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }
}
