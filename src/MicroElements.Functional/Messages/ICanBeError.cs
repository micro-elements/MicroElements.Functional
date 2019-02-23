namespace MicroElements.Functional
{
    /// <summary>
    /// Represents object that can be treated as Error.
    /// </summary>
    public interface ICanBeError
    {
        /// <summary>
        /// Object is Error.
        /// </summary>
        bool IsError { get; }
    }
}
