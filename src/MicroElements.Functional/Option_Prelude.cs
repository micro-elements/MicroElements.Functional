namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        public static Option<T> Some<T>(T value) => new Some<T>(value); // wrap the given value into a Some
        public static None<T> None<T>() => default(None<T>);  // the None value
    }
}
