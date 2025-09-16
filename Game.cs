using System;
using System.Collections.Generic;
using System.Data;
using System.Security;

namespace ScatterBrainersV2
{
    /*
     * 2nd attempt at this text-based game! 8/29/25 
     * This is where the app starts 
     */

    public class Game
    {
        // private instance fields, only accessible inside Game class; each instance of
        // Game created gets its own set of these; the fields are variables declared 
        // directly in the class, outside of method or constructor
        private Player _player;
        private Parser _parser;
        private bool _playing;

        public Game()
        {
            // instance fields are initialized here in the constructor
            _playing = false; // defaults to false, flag will be updated in Start and End methods
            _parser = new Parser(new CommandWords()); // turns raw player input strings into command objects
            _player = new Player(GameWorld.Instance.Entrance); 
        }

        // Main play routine. Loops until end of play.

        public void Play()
        {

            // Enter main command loop. Read commands and execute until game over.
            while (_playing)
            {
                Console.Write("\n>");
                String? entry = Console.ReadLine(); // nullable variable

                if (!string.IsNullOrWhiteSpace(entry))
                {
                    // ParseCommand looks up command keyword, attaches second word (if any), returns object
                    Command command = _parser.ParseCommand(entry); // entry is raw text the player types

                    if (command == null) // if command not entered or not recognized
                    {
                        _player.ErrorMessage("I don't understand...");
                    }
                    else
                    {
                        // if command recognized, it's executed here
                        bool wannaQuit = command.Execute(_player); // all commands return false but QuitCommand
                        if (wannaQuit)
                        {
                            _playing = false;
                        }
                    }
                }
            }
        }

        public void Start() // sets status of playing to allow beginning
        {
            _playing = true;
            _player.InfoMessage(Welcome());
        }

        public void End() 
        {
            _player.InfoMessage(Goodbye());
        }

        public string Welcome()
        {
            return "Welcome to Scatter-Brainers! a new, only slightly boring adventure game.\n\nType 'help' for help." + _player.CurrentRoom.Description();
        }

        public string Goodbye()
        {
            return "\nThank you for playing! Goodbye. \n";
        }
    }
}