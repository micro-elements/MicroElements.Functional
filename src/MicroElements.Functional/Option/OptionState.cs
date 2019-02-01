namespace MicroElements.Functional
{
    /// <summary>
    /// Represents Option state: Some | None.
    /// </summary>
    public enum OptionState : byte
    {
        /// <summary>
        /// Option is None.
        /// </summary>
        None,

        /// <summary>
        /// Option is Some (has value).
        /// </summary>
        Some,
    }
}