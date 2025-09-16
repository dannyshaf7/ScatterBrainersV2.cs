using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    // design pattern - Decorator - dynamically adds behavior to objects by layering them or wrapping them with functionality
    public class Container : IItemContainer, IClosable, ILockable
    {
        private Dictionary<string, List<IItem>> _items;
        private int _numItems;
        private String _name;
        public String Name { get { return _name; } }
        private float _weight;
        public float Weight { get { return _weight; } }
        public String LongName { get { return Name; } }
        private bool _open;
        public bool IsOpen { get { return _open; } }
        public bool IsClosed { get { return !_open; } }
        private ILockable? _lock; // allows nullable 
        public ILockable? TheLock { set { _lock = value; } get { return _lock; } } // allows nullable
        private IItem? _decorator; // allows nullable
        public Stack<IItem> decoratorStack = new Stack<IItem>();
        public IItem? CurrentDecorator => decoratorStack.Count > 0 ? decoratorStack.Peek() : null;
        private float _capacity;
        public float Capacity { get { return _capacity; } set { _capacity = value; } }
        public bool IsLocked { get { return _lock == null ? false : _lock.IsLocked; } }
        public bool IsUnlocked { get { return _lock == null ? true : _lock.IsUnlocked; } }
        public bool IsPowerup { get; }

        public String Description
        {
            get
            {
                String output = "";
                if (_items != null)
                {
                    if (this.Name.Equals("floor"))
                    {
                        output = "You see scattered on the floor: ";
                    }
                    else
                    {
                        output = this.Name + " contents: ";
                    }
                    foreach (List<IItem> itemList in _items.Values)
                    {
                        foreach (IItem item in itemList)
                        {
                            output += " " + item.Name;
                        }
                    }
                }
                return output;
            }
        }

        public Container() : this("No Container Name", 1f, null) { }
        public Container(string name) : this(name, 1f, null) { }
        // need to add more default constructors to cover all parameters

        //Designated Constructor
        public Container(string name, float weight, ILockable? lockable)
        {
            _name = name;
            _weight = weight;
            _open = false;
            _items = new Dictionary<string, List<IItem>>();
            _capacity = 999.9f;
            _lock = lockable;
        }

        public void AddDecorator(IItem decorator) // passes item that will become a decorator
        {
            Capacity += decorator.Capacity;
            if (_decorator == null) // if there is not already a decorator
            {
                _decorator = decorator; // item passed becomes decorator
                decoratorStack = new Stack<IItem>();
                decoratorStack.Push(_decorator);
            }
            else
            {
                Console.WriteLine("You are already holding " + _decorator + ". Please drop it to pick this up.");
            }
        }

        public void RemoveDecorator()
        {
            if (decoratorStack != null && decoratorStack.Count > 0)
            {
                var removed = decoratorStack.Pop();
                Capacity -= removed.Capacity;

                if (decoratorStack.Count == 0)
                {
                    _decorator = null;
                }
                else
                {
                    _decorator = decoratorStack.Peek();
                }
            }
        }

        // how do I get the list to also keep track of number of items inside it? Count?
        // _items[item.Name].Count can be used for this
        public bool Add(IItem item)
        {
            bool success = false;
            if (item.IsPowerup)
            {
                this.AddDecorator(item);
            }
            if (!_items.ContainsKey(item.Name))
            {
                _items[item.Name] = new List<IItem>(); // creates new list in items dictionary using key of item name
                _items[item.Name].Add(item); // adds a new item to the list found by key of item's name
                success = true;
            }
            else
            {
                _items[item.Name].Add(item);
                success = true;
            }
            return success;
        }

        public IItem Get(string itemName)
        {
            IItem tempItem = null;
            if (_items.ContainsKey(itemName))  // if dictionary of lists has provided item name as a key, item already has a list in container
            {
                List<IItem> tempList = _items[itemName]; // value of the dictionary with key of passed item name, list assigned to new variable list
                tempItem = tempList[tempList.Count - 1];
                tempList.RemoveAt(tempList.Count - 1);   // IItem object at the end of the list position removed from list
                if (tempList.Count == 0)     // if there are no IItem objects left on the list
                {
                    _items.Remove(itemName);  // dictionary entry, list of same type items itself is removed from dictionary of items in container
                }
            }
            return tempItem;
        }

        public String GetItems() //so i need to get each dictionary entry first?
        {
            string itemNames = "Items: ";
            Dictionary<string, List<IItem>>.KeyCollection keys = _items.Keys;
            foreach (string itemName in keys)
            {
                itemNames += " " + itemName;
            }
            return itemNames;
        }

        public Dictionary<string, List<IItem>>.KeyCollection GetContents()
        {
            Dictionary<string, List<IItem>>.KeyCollection keys = _items.Keys;
            return keys;
        }

        public bool Open()
        {
            // if there is no lock (null) open is true, if there is open method is run
            _open = _lock == null ? true : _lock.OnOpen();
            return _open;
        }

        public bool Close()
        {
            _open = false; // always close even if lock is set
            _lock?.OnClose(); // if lock exists, call method to notify lock, don't depend on it for closing
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
    }
}