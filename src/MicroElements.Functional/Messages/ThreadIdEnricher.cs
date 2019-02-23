using System.Threading;

namespace MicroElements.Functional
{
    /// <summary>
    /// Adds "ThreadId" property.
    /// </summary>
    public class ThreadIdEnricher : IMessageEnricher
    {
        /// <inheritdoc />
        public IMessage Enrich(IMessage message) =>
            message.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId);
    }
}
