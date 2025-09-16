using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    // This is a singleton design pattern - only one instance of a class can exists during life of progam
    // Also the Observer design pattern - observers and subscribers
    public class NotificationCenter // public-facing class that manages everything, registry of all notifications
    {
        private Dictionary<String, EventContainer> observers;
        private static NotificationCenter? _instance;
        public static NotificationCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationCenter();
                }
                return _instance;
            }
        }

        public NotificationCenter()
        {
            observers = new Dictionary<String, EventContainer>();
        }

        // inner class - private helper, each notification name in dictionary maps to an eventcontainer
        // the EventContainer manages that one notification's subscribers, subscription list for notification
        private class EventContainer
        {
            // methods work on event delegate list for one notification type
            // events are multicast delegate fields, compiler sees Observer as default non-nullable
            // here, the Observer starts with no subscribers (null)
            private event Action<Notification>? Observer; // allows nullable observer
            public EventContainer() { }

            public void AddObserver(Action<Notification> observer)
            {
                Observer += observer;
            }
            public void RemoveObserver(Action<Notification> observer)
            {
                Observer -= observer;
            }
            public void SendNotification(Notification notification)
            {
                Observer?.Invoke(notification); // null-conditional invocation, if no subscribers nothing happens
            }

            public bool IsEmpty()
            {
                return Observer == null;
            }
        }

        // methods work at dictionary level, finding right EventContainer by notification name, delegates work
        public void AddObserver(String notificationName, Action<Notification> observer)
        {
            if (!observers.ContainsKey(notificationName))
            {
                observers[notificationName] = new EventContainer();
            }
            observers[notificationName].AddObserver(observer);
        }
        public void RemoveObserver(String notificationName, Action<Notification> observer)
        {
            if (observers.ContainsKey(notificationName))
            {
                observers[notificationName].RemoveObserver(observer);
                if (observers[notificationName].IsEmpty())
                {
                    observers.Remove(notificationName);
                }
            }
        }
        public void PostNotification(Notification notification)
        {
            if (observers.ContainsKey(notification.Name))
            {
                observers[notification.Name].SendNotification(notification);
            }
        }
    }
}