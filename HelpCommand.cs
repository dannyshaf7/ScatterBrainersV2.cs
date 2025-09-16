using System; 

namespace ScatterBrainersV2
{
    public class HelpCommand : Command
    {
        private CommandWords _words;

        // Designated Constructor - must always be given a CommandWords object
        public HelpCommand(CommandWords commands) : base()
        {
            // bug prevention if someone tries new HelpCommand(null)
            _words = commands ?? throw new ArgumentNullException(nameof(commands));
            this.Name = "help"; // sets name property so the dictionary can recognize it
        }

        override
        public bool Execute(Player player)
        {
            // help command does not anticipate a second word, warning printed if received
            if (this.HasSecondWord())
            {
                player.WarningMessage("\nI cannot help you with " + this.SecondWord);
            }
            else
            {
                // help info message prints and lists the available commands
                player.InfoMessage("\nYou are lost. You are alone. You wander around your home. \n\nYour available commands are: " + _words.Description());
            }
            return false;
        }
    }
}