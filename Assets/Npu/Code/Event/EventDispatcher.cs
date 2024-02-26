namespace Npu
{

    public class EventDispatcher : PriorityEventDispatcher<EventType, object, EventTypeComparer>
    {
        static EventDispatcher instance = new EventDispatcher();

        public static EventDispatcher Instance
        {
            get { return instance; }
        }

    }
}