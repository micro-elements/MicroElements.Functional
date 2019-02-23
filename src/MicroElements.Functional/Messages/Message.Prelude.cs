namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        public static Message ErrorMessage(string text) => new Message(text, severity: MessageSeverity.Error);
        public static Message WarningMessage(string text) => new Message(text, severity: MessageSeverity.Warning);
        public static Message InformationMessage(string text) => new Message(text, severity: MessageSeverity.Information);
    }
}