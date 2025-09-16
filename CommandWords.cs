using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class CommandWords
    {
        private Dictionary<string, Command> _commands; // lookup table keys are command names, values are command objects
        // HelpCommand() is not added here, it's added in the constructor
        private static Command[] _commandArray =
        [
            new BackCommand(), new CloseCommand(), new DropCommand(), new ExamineCommand(),
            new GoCommand(), new LockCommand(), new OpenCommand(),
            new PlaceCommand(), new QuitCommand(), new TakeCommand(), new UnlockCommand()
        ];

        // constructor initializer - public CommandWords() is the default parameterless constructor
        // ": this(_commandArray)" calls designated constructor with default commands
        // functionally, if someone doesn't pass commands, use default list and reuse other constructor
        public CommandWords() : this(_commandArray) { }

        // Designated Constructor
        public CommandWords(Command[] commandList)
        {
            // new dictionary created to add the list of commands passed on CommandWords creation
            _commands = new Dictionary<string, Command>();
            foreach (Command command in commandList)
            {
                _commands[command.Name] = command;
            }
            // this refers to the current instance of the class, so help will always show the 
            // actual set of available commands built for the game
            Command help = new HelpCommand(this); 
            _commands[help.Name] = help;
        }

        public Command? Get(string word) // method expects the name of the command
        {
            // looks through the dictionary for the named command, if found puts Command object
            // into new command variable and if not found returns null (not a valid command)
            if (_commands.TryGetValue(word, out var command))
            {
                return command;
            }
            return null; // command not found
        }

        public string Description() // returns a comma, space-separated string of all command names
        {
            // .Keys returns all the keys in the dictionary (command names in this app)
            return string.Join(", ", _commands.Keys);
        }
    }
}