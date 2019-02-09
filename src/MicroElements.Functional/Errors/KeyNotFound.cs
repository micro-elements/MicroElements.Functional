namespace MicroElements.Functional.Errors
{
    /// <summary>
    /// Represents KeyNotFound error.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    public class KeyNotFound<TKey> : KeyNotFound
    {
        /// <summary>
        /// Key.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Creates new instance of <see cref="KeyNotFound{TKey}"/>.
        /// </summary>
        /// <param name="key">Key.</param>
        public KeyNotFound(TKey key)
            : base(null, "Key {0} is not exists in dictionary.", key) =>
            Key = key;
    }

    /// <summary>
    /// Represents KeyNotFound error.
    /// </summary>
    public abstract class KeyNotFound : BaseError
    {
        protected KeyNotFound(string errorCode, string errorFormat, params object[] args) : base(errorCode, errorFormat, args)
        {
        }
    }
}
