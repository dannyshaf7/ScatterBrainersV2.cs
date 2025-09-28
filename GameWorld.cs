using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        public Room? playerRoom; // nullable - playerRoom is always set via PlayerEnteredRoom
        public Container currentContainer = null!;
        public Container targetContainer = null!;
        private Room exit = null!;
        public GameClock _clock;
        public GameClock Clock { get { return _clock; } }
        private bool _burglarTimeout;
        public bool BurglarTimeout { get { return _burglarTimeout; } }
        private List<Item> _targetItems;

        private GameWorld()
        {
            _targetItems = new List<Item>();
            _entrance = CreateWorld();
            _burglarRoom = _entrance;
            burglar = new Burglar(_burglarRoom); // burglar instance
            burglarAdapter = new BurglarAdapter(burglar); // wrapper adapting the burglar instance
            _burglarTimeout = true;
            _clock = new GameClock(1000);
            NotificationCenter.Instance.AddObserver("PlayerEnteredRoom", PlayerEnteredRoom);
            NotificationCenter.Instance.AddObserver("GameClockTick", GameClockTick);
            NotificationCenter.Instance.AddObserver("PlayerPlacedItem", PlayerPlacedItem);
            NotificationCenter.Instance.AddObserver("PlayerOpenedContainer", PlayerOpenedContainer);
        }

        public void PlayerEnteredRoom(Notification notification)
        {
            Console.WriteLine($"DEBUG: currentContainer is {(currentContainer == null ? "null" : currentContainer.Name)}");

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
            // pattern matching syntax - avoids casting twice - check whether notification.Object 
            // isn't an Item and if it is, assign the casted value to new variable item and 
            // immediately return, guards agains the wrong object type coming in
            if (notification.Object is not Item item) return;

            if (playerRoom?.RoomTag == targetRoom?.RoomTag) // compare room tags (names)
            {
                // string interpolation - $ tells app that string has placeholders "{ }" and replace
                // them with variable values (value of playerRoom.RoomTag), what's inside the braces
                // will be evaluated as code, not a literal part of the string
                Console.WriteLine($"Target room reached: {playerRoom?.RoomTag}");
                Console.WriteLine($"Player container is: {currentContainer?.Name}");

                if (currentContainer != null && currentContainer.Name == targetContainer?.Name) // compare container names
                {
                    Console.WriteLine($"Target container opened: {currentContainer.Name}");

                    // look for match by name - LINQ loops through targetItems list from index 0 forward
                    // for each i, condition (is Name the same) is checked and first match is returned
                    // and assigned to match variable. FirstOrDefault LINQ extension method retrieves the
                    // first in a sequence that satisfies a condition or default (null) if no match found.
                    // => is lambda operator, separates input parameters (left) from expression (right)
                    var match = _targetItems?.FirstOrDefault(i => i.Name == item.Name);
                    if (match != null)
                    {
                        Console.WriteLine($"Target item placed: {match.Name}");
                        _targetItems.Remove(match);

                        if (_targetItems.Count <= 0)
                        {
                            Console.WriteLine("You collected and placed all target items!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Placed item {item.Name}, but it's not on the target list.");
                    }
                }
                else
                {
                    // currentContainer?.Name safely tries to get container name. If null, returns null instead of crashing.
                    // If the left side is null, ?? checks and replaces with string none. same for targetContainer
                    string currentContainerName = currentContainer?.Name ?? "none";
                    string targetContainerName = targetContainer?.Name ?? "none";
                    Console.WriteLine($"Wrong container. Player: {currentContainerName}, Target: {targetContainerName}");
                }
            }
        }

        public void PlayerOpenedContainer(Notification notification)
        {
            
            if (notification.Object is not Container container) return;
            currentContainer = container;
            // Console.WriteLine($"DEBUG: PlayerOpenedContainer called for {container.Name}");
        }

        public void ShowTargets()
        {
            if (_targetItems.Count == 0)
            {
                Console.WriteLine("All target items have been placed!");
                return;
            }

            Console.WriteLine("Target items remaining:");

            // multi-line syntax is chaining of LINQ methods, calls both _targetItems.GroupBy(...) and 
            // _targetItems.Select(...) LINQ methods for handling data in memory. The LINQ extension
            // methods return new enumerable sequences that can be chained one after another
            var grouped = _targetItems
                // groups all items by their name, returns an IEnumerable<IGrouping<string, Item>>
                .GroupBy(i => i.Name)
                // transforms each group into new object with a Name and a Count
                .Select(g => new { Name = g.Key, Count = g.Count() });

            foreach (var entry in grouped)
            {
                Console.WriteLine($" - {entry.Name} x{entry.Count}");
            }
        }


        public Room CreateWorld()
        {
            Room outside = new("on the porch outside the front door");
            Room entryWay = new("in the entryway of the home");
            Room hallWay = new("in the main hallway of the home");
            Room livingRoom = new("in the living room");
            Room kitchen = new("in the kitchen");
            Room halfBath = new("in the half bathroom");
            Room laundryRoom = new("in the laundry room");
            Room masterBedroom = new("in the master bedroom");
            Room masterBath = new("in the master bathroom");
            Room office = new("in the office");
            Room smallBedroom = new("in the small bedroom");

            // door variable is needed to be able to attach a lock
            Door frontDoor = Door.ConnectRooms("north", "south", outside, entryWay);
            frontDoor.TheLock = new RegularLock();
            Door.ConnectRooms("north", "south", entryWay, kitchen);
            Door.ConnectRooms("east", "west", entryWay, livingRoom);
            Door.ConnectRooms("north", "south", hallWay, masterBedroom);
            Door officeDoor = Door.ConnectRooms("north", "south", office, hallWay);
            officeDoor.TheLock = new RegularLock();
            Door.ConnectRooms("east", "west", hallWay, kitchen);
            Door.ConnectRooms("east", "west", smallBedroom, hallWay);
            Door.ConnectRooms("north", "south", livingRoom, halfBath);
            Door.ConnectRooms("east", "west", kitchen, laundryRoom);
            Door.ConnectRooms("east", "west", masterBath, masterBedroom);

            Container kitchenTable = new("kitchen-table");
            Container pantry = new("pantry");
            Container fridge = new("refridgerator");
            Container kitchenSink = new("kitchen-sink");
            Container entryCloset = new("entry-closet");
            Container masterChest = new("chest-of-drawers", 50f, new RegularLock());
            Container masterBathSink = new("master-bath-sink");
            Container desk = new("desk", 30f, new RegularLock());
            Container washer = new("washer");

            entryWay.SetContainer(entryCloset);
            kitchen.SetContainer(kitchenTable);
            kitchen.SetContainer(fridge);
            kitchen.SetContainer(pantry);
            kitchen.SetContainer(kitchenSink);
            office.SetContainer(desk);
            masterBedroom.SetContainer(masterChest);
            masterBath.SetContainer(masterBathSink);
            laundryRoom.SetContainer(washer);

            Item fork = new("fork", 0.1f);
            Item spoon = new("spoon", 0.1f);
            Item spatula = new("spatula", 0.2f);
            Item butcherKnife = new("butcher-knife", 0.2f);
            Item cup = new("cup", 0.3f);
            Item plate = new("plate", 0.4f);
            Item bowl = new("bowl", 0.4f);
            Item carKeys = new("carkeys", 0.5f);
            Item cellPhone = new("cellphone", 0.5f);
            Item wallet = new("wallet", 0.5f);
            Item milk = new("milk", 3.0f);
            Item banana = new("banana", 0.5f);
            Item apple = new("apple", 0.5f);
            Item plunger = new("plunger", 1.0f);
            Item bowlingBall = new("bowlingball", 5.0f);
            Item umbrella = new("umbrella", 1.5f);
            Item clothes = new("clothes", 1.0f);
            Item socks = new("socks", 0.2f);
            Item pen = new("pen", 0.1f);
            Item candle = new("candle", 0.4f);
            Item lamp = new("lamp", 2.5f);
            Item guitar = new("guitar", 3.5f);
            Item vacuum = new("vacuum", 4.0f);
            Item giantTeddyBear = new("giantteddybear", 5.0f);
            Item purse = new("purse", 0.0f)
            {
                IsPowerup = true,
                Capacity = 2.0f
            };
            Item backpack = new("backpack", 0.0f)
            {
                IsPowerup = true,
                Capacity = 5.0f
            };
            Item tote = new("tote", 0.0f)
            {
                IsPowerup = true,
                Capacity = 3.0f
            };

            entryCloset.Add(tote);
            entryCloset.Add(purse);
            entryCloset.Add(backpack);
            entryCloset.Add(umbrella);
            entryCloset.Add(vacuum);
            entryCloset.Add(lamp);

            livingRoom.Floor.Add(clothes);

            kitchenSink.Add(fork);
            kitchenSink.Add(fork);
            kitchenSink.Add(spoon);
            kitchenSink.Add(spoon);
            kitchenSink.Add(spoon);
            kitchenSink.Add(cup);
            kitchenSink.Add(bowl);
            kitchenSink.Add(butcherKnife);
            kitchenSink.Add(plate);
            kitchenSink.Add(spatula);
            kitchenTable.Add(cellPhone);
            kitchenTable.Add(apple);
            kitchenTable.Add(apple);
            kitchenTable.Add(apple);
            kitchenTable.Add(apple);
            kitchenTable.Add(banana);
            kitchenTable.Add(banana);
            kitchenTable.Add(banana);
            kitchenTable.Add(banana);
            kitchenTable.Add(banana);
            kitchenTable.Add(banana);
            fridge.Add(milk);
            fridge.Add(milk);

            smallBedroom.Floor.Add(bowlingBall);
            smallBedroom.Floor.Add(guitar);
            smallBedroom.Floor.Add(socks);
            smallBedroom.Floor.Add(giantTeddyBear);

            desk.Add(carKeys);
            desk.Add(pen);
            masterChest.Add(fork);
            masterChest.Add(wallet);

            masterBath.Floor.Add(clothes);
            masterBath.Floor.Add(plunger);
            masterBathSink.Add(candle);

            targetRoom = laundryRoom;
            targetContainer = washer;
            _targetItems.Add(new Item("clothes", 1.0f));
            _targetItems.Add(new Item("clothes", 1.0f));

            return outside;
        }
    }
}