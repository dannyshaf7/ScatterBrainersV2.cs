using System;
using System.Collections.Generic;
using System.Text;
using System.Timers; // ElapsedEventHandler is defined in .NET framework

namespace ScatterBrainersV2
{
    // game clock ticks every interval of milliseconds
    public class GameClock
    {
        private System.Timers.Timer timer;
        private int _timeInGame;
        public int TimeInGame { get { return _timeInGame; } }

        public GameClock(int interval)
        {
            timer = new System.Timers.Timer(interval); // timer class declares event that uses the delegate
            timer.Elapsed += OnTimedEvent; // when timer ticks, elapsed fires, OnTimedEvent is called
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // delegates define what a handler method must look like, how events work in .NET
        // compiler looks at Elapsed event, method matches delegate's signature (contract)
        private void OnTimedEvent(object? source, ElapsedEventArgs e) // needs ? nullable to match delegate
        {
            _timeInGame++; // time in game is incremented when called, tracks passage of time
            Notification notification = new Notification("GameClockTick", this);
            NotificationCenter.Instance.PostNotification(notification);
        }
    }
}