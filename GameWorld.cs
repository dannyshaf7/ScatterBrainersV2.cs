using System;
using System.Collections.Generic;
using System.Numerics;

namespace ScatterBrainersV2
{
    internal class GameWorld
    {
        private static GameWorld? _instance = null;
        public static GameWorld Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new GameWorld();
                }
                return _instance;
            }
        }

        public Burglar burglar; 
        public BurglarAdapter burglarAdapter;
        private Room _entrance;
        public Room Entrance { get { return _entrance; } }
        private Room? _burglarRoom;
        public Room targetRoom = null!; // ! is null-forgiving operator, allowing it to be set later
        public Room playerRoom = new Room();
        public Container playerContainer = null!;
        public Container targetContainer = null!;
        private Room exit = null!;
        public GameClock _clock;
        public GameClock Clock { get { return _clock; } }
        private bool _burglarTimeout;
        public bool BurglarTimeout { get { return _burglarTimeout; } }
        private Stack<Item> _targetItems;

        private GameWorld()
        {
            _targetItems = new Stack<Item>();
            _entrance = CreateWorld();
            _burglarRoom = _entrance;
            burglar = new Burglar(_burglarRoom); // burglar instance
            burglarAdapter = new BurglarAdapter(burglar); // wrapper adapting the burglar instance
            _burglarTimeout = true;
            _clock = new GameClock(1000);
            NotificationCenter.Instance.AddObserver("PlayerEnteredRoom", PlayerEnteredRoom);
            NotificationCenter.Instance.AddObserver("GameClockTick", GameClockTick);
            NotificationCenter.Instance.AddObserver("PlayerPlacedItem", PlayerPlacedItem);
        }

        public void PlayerEnteredRoom(Notification notification)
        {
            // pattern matching, checks the condition and declares the variable
            if (notification.Object is not Player player) 
            {
                Console.WriteLine("invalid notification for player entered room");
                return;
            }
            playerRoom = player.CurrentRoom ?? throw new InvalidOperationException("Player has no current room.");
            player.InfoMessage(playerRoom.Description());
            string direction = _burglarRoom.RandomRoom();
            
            if (!BurglarTimeout)
            {
                burglarAdapter.WalkTo(direction);
                player.InfoMessage("The burglar is " + burglar.CurrentRoom.RoomTag);
                if (burglar.CurrentRoom != null && burglar.CurrentRoom.Equals(player.CurrentRoom))
                {
                    _burglarTimeout = true;
                    player.WarningMessage("Oh no! The burglar caught you. You lose!");
                }
                _burglarRoom = burglar.CurrentRoom;
            }
        }

        public void GameClockTick(Notification notification)
        {
            GameClock gameClock = (GameClock)notification.Object;
            if (gameClock != null)
            {
                if (Clock.TimeInGame >= 60)
                {
                    _burglarTimeout = false;
                }
            }
        }

        public void PlayerPlacedItem(Notification notification)
        {
            Item item = (Item)notification.Object;
            if (item != null)
            {
                if (playerRoom == targetRoom)
                {
                    Console.WriteLine("Target room reached");
                    if (playerContainer == targetContainer)
                    {
                        Console.WriteLine("Target container opened");
                        Item result = null;
                        _targetItems.TryPeek(out result);
                        if (notification.UserInfo.ContainsValue(result))
                        {
                            Console.WriteLine("target item placed.");
                            _targetItems.Pop();
                            if (_targetItems.Count <= 0)
                            {
                                Console.WriteLine("You collected and placed all target items!");
                            }
                        }
                    }
                }
            }
        }

        public void PlayerOpenedContainer(Notification notification)
        {
            Container container = (Container)notification.Object;
            if (container != null)
            {
                playerContainer = container;
                Console.WriteLine(container.Name);
            }
        }

        public Room CreateWorld()
        {
            Room outside = new Room("on the porch outside the front door");
            Room entryWay = new Room("in the entryway of the home");
            Room hallWay = new Room("in the main hallway of the home");
            Room livingRoom = new Room("in the living room");
            Room kitchen = new Room("in the kitchen");
            Room halfBath = new Room("in the half bathroom");
            Room laundryRoom = new Room("in the laundry room");
            Room masterBedroom = new Room("in the master bedroom");
            Room masterBath = new Room("in the master bathroom");
            Room office = new Room("in the office");
            Room smallBedroom = new Room("in the small bedroom");

            Door door = Door.ConnectRooms("north", "south", outside, entryWay);
            RegularLock aLock = new RegularLock();
            door.TheLock = aLock;

            door = Door.ConnectRooms("north", "south", entryWay, kitchen);
            door = Door.ConnectRooms("east", "west", entryWay, livingRoom);
            door = Door.ConnectRooms("north", "south", hallWay, masterBedroom);
            door = Door.ConnectRooms("north", "south", office, hallWay);
            door = Door.ConnectRooms("east", "west", hallWay, kitchen);
            door = Door.ConnectRooms("east", "west", smallBedroom, hallWay);
            door = Door.ConnectRooms("north", "south", livingRoom, halfBath);
            door = Door.ConnectRooms("east", "west", kitchen, laundryRoom);
            door = Door.ConnectRooms("east", "west", masterBath, masterBedroom);

            Container kitchenTable = new Container("kitchen-table");
            Container entryCloset = new Container("entry-closet");
            Container masterChest = new Container("chest-of-drawers", 50f, new RegularLock());
            Container desk = new Container("desk", 30f, new RegularLock());
            Container fridge = new Container("refridgerator");
            Container washer = new Container("washer");

            entryWay.SetContainer(entryCloset);
            kitchen.SetContainer(kitchenTable);
            kitchen.SetContainer(fridge);
            office.SetContainer(desk);
            masterBedroom.SetContainer(masterChest);
            laundryRoom.SetContainer(washer);

            Item fork = new Item("fork", 0.1f);
            Item purse = new Item("purse", 0.0f);
            purse.IsPowerup = true;
            purse.Capacity = 2.0f;
            Item backpack = new Item("backpack", 0.0f);
            backpack.IsPowerup = true;
            backpack.Capacity = 5.0f;
            Item tote = new Item("tote", 0.0f);
            tote.IsPowerup = true;
            tote.Capacity = 3.0f;
            Item carKeys = new Item("carkeys", 0.5f);
            Item cellPhone = new Item("cellphone", 0.5f);
            Item wallet = new Item("wallet", 0.5f);
            Item milk = new Item("milk", 3.0f);
            Item clothes = new Item("clothes", 1.0f);

            entryCloset.Add(tote);
            entryCloset.Add(purse);
            entryCloset.Add(backpack);
            masterBath.Floor.Add(clothes);
            livingRoom.Floor.Add(clothes);
            kitchenTable.Add(fork);
            kitchenTable.Add(fork);
            kitchenTable.Add(cellPhone);
            masterChest.Add(wallet);
            fridge.Add(milk);
            fridge.Add(milk);
            desk.Add(carKeys);
            masterChest.Add(fork);

            targetRoom = laundryRoom;
            targetContainer = washer;
            _targetItems.Push(clothes);
            _targetItems.Push(clothes);

            return outside;
        }
    }
}