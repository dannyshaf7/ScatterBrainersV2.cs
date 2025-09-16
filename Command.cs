using System; 
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    // design pattern Command -  encapsulates player actions as objects to queue, log, undo them uniformly
    public abstract class Command
    {
        private string _name = "";
        public string Name { get { return _name; } set { _name = value; } }
        private string? _secondWord; // ? makes it explicit that the variable's value can be null
        public string? SecondWord { get { return _secondWord; } set { _secondWord = value; } }

        public Command()
        {
            this.Name = "";
            this.SecondWord = null;
        }

        public bool HasSecondWord()
        {
            return !string.IsNullOrEmpty(this.SecondWord); // checks for null or empty SecondWord variable value
        }

        public abstract bool Execute(Player player);
    }
}