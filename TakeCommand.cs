using System; 

namespace ScatterBrainersV2
{
    public class TakeCommand : Command
    {
        public TakeCommand() : base()
        {
            this.Name = "take";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.TakeItem(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nTake What?");
            }
            return false;
        }
    }
}