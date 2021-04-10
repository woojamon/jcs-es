using System;
namespace Persistence
{
    public class Event<A>
    {
        public Event(
            Guid eventId,
            A payload)
        {
            EventId = eventId;
            Payload = payload;
        }

        public Guid EventId { get; }
        public A Payload { get; }
    }
}
