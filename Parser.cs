using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class Parser
    {
        private CommandWords _commands;
        public Parser() : this(new CommandWords()) { }

        // Designated Constructor
        public Parser(CommandWords newCommands)
        {
            _commands = newCommands;
        }

        public Command ParseCommand(string commandString)
        {
            Command command = null;
            char spaceChar = ' ';
            string[] words = commandString.Split(spaceChar);
            if (words.Length > 0)
            {
                command = _commands.Get(words[0]);
                if (command != null)
                {
                    if (words.Length > 1)
                    { 
                        command.SecondWord = words[1];
                    }
                    else
                    {
                        command.SecondWord = null;
                    }
                }
                else
                {
                    // This is debug code, consider removing for regular execution
                    Console.WriteLine(">>>Didn't find command " + words[0]);
                }
            }
            else
            {
                // This is debug code, consider removing for regular execution
                Console.WriteLine("No words parsed!");
            }
            return command;
        }

        public string Description()
        {
            return _commands.Description();
        }
    }
}