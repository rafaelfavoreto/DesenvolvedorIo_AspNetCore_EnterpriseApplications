using MediatR;

namespace NerdStore.Core.Messagens
{
    public abstract class Event : Message, INotification
    {
        public DateTime Timestamp { get; protected set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
