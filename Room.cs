using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class Room
    {
        private Dictionary<string, Door> _exits;
        private string _roomTag = "";
        public string RoomTag { get { return _roomTag; } set { _roomTag = value; } }
        private Dictionary<string, IItemContainer> _containers;
        private string _containerTag = ""; // I don't think I'm using this yet, i'll keep it and come back later
        public string ContainerTag { get { return _containerTag; } set { _containerTag = value; } }
        private IItemContainer _floor;
        public IItemContainer Floor { get { return _floor; } }
        
        public Room() : this("No Tag") { }
        // Designated Constructor
        public Room(string tag)
        {
            _exits = new Dictionary<string, Door>();
            this.RoomTag = tag;
            _containers = new Dictionary<string, IItemContainer>();
            _floor = new Container("floor");
        }

        public void SetExit(string exitName, Door door)
        {
            _exits[exitName] = door;
        }

        public void SetContainer(IItemContainer container)
        {
            _containers[container.Name] = container;
        }

        public Door? GetExit(string exitName) 
        {
            if (_exits.TryGetValue(exitName, out var door))
            {
                return door;
            }
            return null; // door not found
        }

        public IItemContainer? GetContainer(string containerName)
        {
            // IItemContainer container = null;
            //_containers.TryGetValue(containerName, out container); // throwing null reference exception _containers was null
            
            // if TryGetValue succeeds (key exists), value assigned to new variable. If key doesn't, container is declared as 
            // whatever the default value for IItemContainer is, null in this case.
            if (_containers.TryGetValue(containerName, out var container)) 
            { 
                return container;
            }
            return null; // container not found
        }

        public string GetExits()
        {
            string exitNames = "Exit(s): ";
            Dictionary<string, Door>.KeyCollection keys = _exits.Keys;
            foreach (string exitName in keys)
            {
                exitNames += " " + exitName;
            }
            return exitNames;
        }

        public string GetContainers()
        {
            string containerNames = "Containers: ";
            Dictionary<string, IItemContainer>.KeyCollection keys = _containers.Keys;
            foreach (string containerName in keys)
            {
                containerNames += " " + containerName;
            }
            return containerNames;
        }

        public IItemContainer GetFloor()
        {
            return Floor;
        }

        public void Drop(IItem item)
        {
            _floor.Add(item);
        }

        public IItem TakeItem(String name)
        {
            IItem tempItem = _floor.Get(name);
            return tempItem;
        }

        public String RandomRoom()
        {
            int numExits;
            int randomNumber;
            List<string> exitNames = new List<string>();
            Dictionary<string, Door>.KeyCollection keys = _exits.Keys;
            foreach (string exitName in keys)
            {
                exitNames.Add(exitName);
            }
            numExits = exitNames.Count;
            Random rnd = new Random();
            randomNumber = rnd.Next(numExits);
            Console.WriteLine(exitNames[randomNumber]);
            return exitNames[randomNumber];
        }

        public string Description()
        {
            return "\nYou are " + this.RoomTag + ".\n" + this.GetExits();
        }
    }
}