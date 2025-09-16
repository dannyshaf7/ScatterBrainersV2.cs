using System;

namespace ScatterBrainersV2
{
    public class Burglar
    {
        private Container _playerContainer;
        public Container PlayerContainer { get { return _playerContainer; } set { _playerContainer = value; } }
        private Room _currentRoom;
        public Room CurrentRoom { get { return _currentRoom; } set { _currentRoom = value; } }

        public Burglar(Room room)
        {
            _currentRoom = room;
            _playerContainer = new Container("burglar");
        }

        public void SearchHome(String direction)
        {
            Door? door = this.CurrentRoom.GetExit(direction); // door may be null from GetExit
            if (door != null)
            {
                if (door.IsUnlocked)
                {
                    if (door.IsOpen)
                    {
                        Room nextRoom = door.RoomOnTheOtherSide(CurrentRoom);
                        CurrentRoom = nextRoom;
                    }
                    else
                    {
                        door.Open();
                        NormalMessage("\nThe burglar has opened the " + direction + " door.");
                        this.SearchHome(direction);
                    }
                }
                else
                {
                    ErrorMessage("\nThe burglar is stuck.");
                }
            }
            else 
            {
                ErrorMessage("\nBurglar door error.");
            }
        }

        public void Attack() { }
        public void StealItem(String itemName) { }// passing the wanted item name. I need to check if the player is in the room of the current container.
        public bool Pillage(String name)
        {
            return false;
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