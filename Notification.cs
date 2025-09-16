using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class Notification
    {
        public String Name { get; set; }
        public Object? Object { get; set; } // Object can be passed as null
        public Dictionary<String, Object>? UserInfo { get; set; } // UserInfo can be passed as null

        // Overloaded, Chained Constructors
        public Notification() : this("NotificationName") { }
        public Notification(String name) : this(name, null) { }
        public Notification(String name, object? obj) : this(name, obj, null) { }

        // Default Constructor
        public Notification(String name, object? obj, Dictionary<String, Object>? userInfo)
        {
            this.Name = name;
            this.Object = obj; 
            this.UserInfo = userInfo;
        }
    }
}