using System;

namespace ScatterBrainersV2
{
    public class RegularLock : ILockable
    {
        private bool _locked;
        private bool _isOpen;
        public bool IsOpen { get { return _isOpen; } }
        public bool IsClosed { get { return !_isOpen;} }
        public bool IsLocked { get { return _locked; } }
        public bool IsUnlocked { get { return !_locked; } }

        public RegularLock()
        {
            _locked = false; // unlocked by default
            _isOpen = false; // closed by default
        }

        public bool Open()
        {
            if (_locked) return false; // can't open if locked
            _isOpen = true;
            return true;
        }
        public bool Close()
        {
            _isOpen = false;
            return true;
        }
        public bool Lock()
        {
            if (_isOpen) return false; // can't lock if open
            _locked = true;
            return true;
        }
        public bool Unlock()
        {
            _locked = false;
            return true;
        }
        public bool OnOpen() // can device be opened? determines behavior of device
        {
            return !_locked; // if not locked, returns true; locked returns false
        }
        public bool OnClose()
        {
            return true; // lock can always close whether it is locked or not
        }
    }
}