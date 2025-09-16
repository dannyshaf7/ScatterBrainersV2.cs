using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ScatterBrainersV2 
{
    /*
     * Notes
     */

    public class Player : ICharacter 
    {
        private Container _playerContainer;
        public Container PlayerContainer { get { return _playerContainer; } set { _playerContainer = value; } }
        private Container? _currentContainer; // nullable
        public Container? CurrentContainer { get { return _currentContainer; } set { _currentContainer = value; } } // nullable
        private Room _currentRoom;
        public Room CurrentRoom { get { return _currentRoom; } set { _currentRoom = value; } }
        public Stack<Room> roomsVisited;

        public Player(Room room) 
        {
            _currentRoom = room;
            _playerContainer = new Container("player");
            _playerContainer.Capacity = 2.0f;
            roomsVisited = new Stack<Room>();
        }

        public void WalkTo(string direction)
        {
            Door door = this.CurrentRoom.GetExit(direction);
            if (door != null)
            {
                if(door.IsOpen)
                {
                    roomsVisited.Push(CurrentRoom);
                    Room nextRoom = door.RoomOnTheOtherSide(CurrentRoom);
                    Notification notification = new Notification("PlayerWillLeaveRoom", this);
                    NotificationCenter.Instance.PostNotification(notification);
                    CurrentRoom = nextRoom;
                    CurrentContainer = null;
                    notification = new Notification("PlayerEnteredRoom", this);
                    NotificationCenter.Instance.PostNotification(notification);
                }
                else 
                {
                    ErrorMessage("\nThe door " + direction + " is closed.");
                }

            }
            else
            {
                ErrorMessage("\nThere is no doorway to the " + direction);
            }
        }

        public void Back()
        {
            if(roomsVisited.Count > 0)
            {
                CurrentRoom = roomsVisited.Pop();
            }
            else
            {
                ErrorMessage("You are at the beginning.");
            }
        }

        public bool Lock(String name)
        {
            bool success = false;
            Door door = this.CurrentRoom.GetExit(name);
            if (door != null)
            {
                if (door.IsUnlocked)
                {
                    if (door.Lock())
                    {
                        InfoMessage("The door is now locked!");
                        success = true;
                    }
                    else
                    {
                        ErrorMessage("The door cannot be locked.");
                    }
                }
                else
                {
                    ErrorMessage("The door is already locked.");
                }
            }
            else
            {
                ErrorMessage("There is no door there.");
            }
            return success;
        }

        public bool Unlock(String name)
        {
            bool success = false;
            Door door = this.CurrentRoom.GetExit(name);
            if (door != null)
            {
                if (door.IsLocked)
                {
                    if (door.Unlock())
                    {
                        InfoMessage("The door is now unlocked!");
                        success = true;
                    }
                    else 
                    {
                        ErrorMessage("The door cannot be unlocked.");
                    }
                }
                else
                {
                    ErrorMessage("The door is already unlocked.");
                }
            }
            else
            {
                ErrorMessage("There is no door there.");
            }
            return success;
        }

        public bool Open(String name)
        {
            bool success = false;
            Container container = (Container)CurrentRoom.GetContainer(name);
            Door door = this.CurrentRoom.GetExit(name);
            if (name.Equals("north") || name.Equals("south") || name.Equals("east") || name.Equals("west"))
            {
                if(door != null)
                {
                    if (door.IsClosed)
                    {
                        door.Open();
                        if (door.IsOpen)
                        {
                            InfoMessage("\nThe door to the " + name + " is now open.");
                            success = true;
                        }
                        else
                        {
                            ErrorMessage("\nThe door to the " + name + " is still closed.");
                        }
                    }
                    else
                    {
                        WarningMessage("The door to the " + name + " is already open.");
                    }
                }
                else
                {
                    ErrorMessage("\nThere is no door to the " + name + ".");
                }
                return success;
            }
            else if (container != null)
            {
                if (container.IsClosed)
                {
                    container.Open();
                    if (container.IsOpen)
                    {
                        IItemContainer tempContainer = CurrentRoom.GetContainer(name);
                        Notification notification = new Notification("PlayerOpenedContainer", tempContainer);
                        Dictionary<string, object> userInfo = new Dictionary<string, object>();
                        userInfo["item"] = name; // what user says is stored as variable with keyword as "word", stored scope of method then gone
                        notification.UserInfo = userInfo; // filling dictionary with user uttered word
                        NotificationCenter.Instance.PostNotification(notification);
                        InfoMessage("\nThe " + container.Name + " is now open.");
                        CurrentContainer = container;
                        success = true;
                    }
                    else
                    {
                        ErrorMessage("The " + container.Name + " is still closed.");
                    }
                }
                else
                {
                    ErrorMessage("The " + container.Name + " is already open.");
                }
            }
            else
            {
                ErrorMessage("\nThere is no " + name + "door or container to open.");
            }
            return success;
        }

        public bool Close(String name) // how do I get the player to close? or should I close box automatically on next cmd?
        {
            bool success = false;
            Container container = (Container)CurrentRoom.GetContainer(name);
            Door door = this.CurrentRoom.GetExit(name);
            if (name.Equals("north") || name.Equals("south") || name.Equals("east") || name.Equals("west"))
            {
                if (door != null)
                {
                    if (door.IsOpen)
                    {
                        door.Close();
                        if (door.IsClosed)
                        {
                            InfoMessage("\nThe door to the " + name + " is now closed.");
                            success = true;
                        }
                        else
                        {
                            ErrorMessage("\nThe door to the " + name + " is still open.");
                        }
                    }
                    else
                    {
                        ErrorMessage("\nThe " + " door is already closed.");
                    }
                }
                else
                {
                    ErrorMessage("\nThere is no door to the " + name + ".");
                    success = false;
                }
                return success;
            }
            else if (container != null)
            {
                if (container.IsOpen)
                {
                    container.Close();
                    if (container.IsClosed)
                    {
                        InfoMessage("\nThe " + container.Name + " is now closed.");
                        CurrentContainer = null;
                        success = true;
                    }
                    else
                    {
                        ErrorMessage("\nThe " + container.Name + " is still open.");
                    }
                }
                else
                {
                    WarningMessage("\nThe " + container.Name + " is already closed.");
                }
            }
            else
            {
                ErrorMessage("\nThere is no " + name + " to close.");
            }
            return success;
        }

        public void TakeItem(String itemName) // passing the item name that's wanted. Check if the player in same room of container.
        {
            if (CurrentContainer != null) // has a container been opened already? If so, is container in the room?
            {
                if (CurrentRoom.GetContainer(CurrentContainer.Name) != null) // how can I take from the floor? maybe in command, if no open container?
                {
                    if (CurrentContainer.IsOpen)
                    {
                        IItem tempItem = CurrentContainer.Get(itemName);
                        if (tempItem != null)
                        {
                            float itemWeight = tempItem.Weight;
                            if (PlayerContainer.Capacity >= itemWeight)
                            {
                                PlayerContainer.Add(tempItem);
                                PlayerContainer.Capacity -= itemWeight;
                                CurrentContainer.Capacity += itemWeight;
                                InfoMessage("You took the " + itemName + " from the " + CurrentContainer.Name + "!");
                            }
                            else
                            {
                                CurrentContainer.Add(tempItem);
                                ErrorMessage("You don't have enough space to hold the " + itemName + ".");
                            }
                        }
                        else
                        {
                            ErrorMessage("There is no " + itemName + " in the " + CurrentContainer.Name + ".");
                        }
                    }
                    else
                    {
                        WarningMessage("The " + CurrentContainer.Name + " is closed.");
                    }
                }
                else
                {
                    ErrorMessage("The open container is not in this room.");
                }
            }
            else     // will this pick up from any floor in the house? checks room in PickUpFromFloor method
            {
                IItem tempItem = PickUpFromFloor(itemName);
                if (tempItem != null)
                {
                    PlayerContainer.Add(tempItem);
                    InfoMessage("You took the " + itemName + " from the floor!");
                }
                else
                { 
                    ErrorMessage("There is no " + itemName + " on the floor. \nYou may also find items in containers throughout the house.");
                }
            }
        }

        public void PlaceItem(String itemName)
        {
            if (CurrentContainer != null)
            {
                if (CurrentContainer.IsOpen)
                {
                    IItem tempItem = PlayerContainer.Get(itemName);
                    CurrentContainer.Add(tempItem);
                    InfoMessage("You placed the " + itemName + " in the " + CurrentContainer.Name + "!");
                    Notification notification = new Notification("PlayerPlacedItem", tempItem);
                    Dictionary<string, object> userInfo = new Dictionary<string, object>();
                    userInfo["item"] = itemName; // what the user says is stored as variable with keyword "word", stored scope of method then gone
                    notification.UserInfo = userInfo; // filling dictionary with one player uttered word
                    NotificationCenter.Instance.PostNotification(notification);
                }
                else
                {
                    WarningMessage("The " + CurrentContainer.Name + " is closed.");
                }
            }
            else
            {
                ErrorMessage("The open container is not in this room.");
            }
        }
        // else
        // {
        //     ErrorMessage("You must open a container first.");
        // }

        public void Drop(String itemName)
        {
            IItem tempItem = PlayerContainer.Get(itemName);
            if (tempItem != null)
            {
                IItemContainer floor = _currentRoom.GetFloor();
                floor.Add(tempItem);
                InfoMessage("You dropped the " + itemName + ".");
            }
            else
            {
                ErrorMessage("You do not hold a " + itemName + " in your inventory.");
            }
        }

        public void DropAll()
        { 
            Dictionary<string, List<IItem>>.KeyCollection keys = PlayerContainer.GetContents();
            foreach (string itemName in keys)
            {
                Drop(itemName);
            }
        }
        
        public IItem PickUpFromFloor(String itemName)
        {
            IItemContainer floor = _currentRoom.GetFloor();
            IItem tempItem = floor.Get(itemName);
            return tempItem;
        }

        public void Examine(String subjectName)
        {
            if (subjectName.Equals("room"))
            {
                InfoMessage(CurrentRoom.Description()); // why isn't this printing out containers in the room?
                InfoMessage(CurrentRoom.GetContainers());
                IItemContainer floor = CurrentRoom.Floor;
                InfoMessage(floor.Description);
            }
            else if (CurrentContainer != null)
            {
                if (CurrentContainer.Name.Equals(subjectName)) // not good enough-can be more than one container open. Need to see if container in room then list.
                {
                    if (CurrentContainer.IsOpen)
                    {
                        InfoMessage(CurrentContainer.GetItems());
                    }
                    else
                    {
                        ErrorMessage("You must open a container to view its contents.");
                    }
                } 
            }
            else
            {
                ErrorMessage("You do not see the " + subjectName);
            }
        }

        public void Inventory()
        {
            String inventoryMessage = "Your carrying capacity is " + _playerContainer.Capacity + " .";
            inventoryMessage += "\n" + _playerContainer.Description;
            NormalMessage(inventoryMessage);
        }

        public void Defend() 
        {
            // plan was to add some battle stuff here
        }

        public void OutputMessage(string message)
        {
            Console.WriteLine(message);
        }
        
        public void ColoredMessage(string message, ConsoleColor newColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            OutputMessage(message);
            Console.ForegroundColor = oldColor;
        }

        public void NormalMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.White);
        }

        public void InfoMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Blue);
        }

        public void WarningMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.DarkYellow);
        }

        public void ErrorMessage(string message)
        {
            ColoredMessage(message, ConsoleColor.Red);
        }
    }
}