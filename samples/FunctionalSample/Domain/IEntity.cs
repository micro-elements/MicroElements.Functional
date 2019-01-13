namespace FunctionalSample.Domain
{
    /// <summary>
    /// Entity in DDD mean.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    public interface IEntity<out TKey>
    {
        /// <summary>
        /// Entity Id.
        /// </summary>
        TKey Id { get; }
    }
}