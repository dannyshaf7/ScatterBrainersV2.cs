using System;

namespace ScatterBrainersV2
{
    public class Door : ILockable
    {
        private readonly Room _room1;
        private readonly Room _room2;
        private bool _open;
        private ILockable? _lock; // allows lock to be null, not all doors will have a lock
        public ILockable? TheLock { set { _lock = value; } get { return _lock; } }
        public bool IsOpen { get { return _open; } }
        public bool IsClosed { get { return !_open; } }
        public bool IsLocked { get { return _lock == null ? false : _lock.IsLocked; } }
        public bool IsUnlocked { get { return _lock == null ? true : _lock.IsUnlocked; } }

        public Door(Room room1, Room room2)
        {
            _lock = null;
            _open = false; // door defaults to closed
            _room1 = room1;
            _room2 = room2;
        }

        public Room RoomOnTheOtherSide(Room from)
        {
            if (from == _room1)
            {
                return _room2;
            }
            else
            {
                return _room1;
            }
        }

        public bool Open()
        {
            _open = _lock == null ? true : _lock.OnOpen();
            return _open;
        }

        public bool Close()
        {
            _open = false; 
            _lock?.OnClose();
            return _open;
        }

        /* 
         * need to address issues with these lock/unlock methods and encorporate them
         * along with the RegularLock object. They are not functional as-is. I don't
         * know if the keys needed to unlock are created yet either
         */


        public bool Lock()
        {
            if (_lock == null) return false;
            return _lock.Lock();  // let lock decide if it succeeded
        }

        public bool Unlock()
        {
            if (_lock != null)
            {
                _lock.Unlock();
                return true;
            }
            return false;
        }

        public bool OnOpen()
        {
            return _lock == null ? true : _lock.OnOpen();
        }

        public bool OnClose()
        {
            return _lock == null ? true : _lock.OnClose();
        }

        public static Door ConnectRooms(String label1, String label2, Room room1, Room room2)
        {
            Door door = new Door(room1, room2);
            room1.SetExit(label1, door);
            room2.SetExit(label2, door);
            return door;
        }
    }
}
