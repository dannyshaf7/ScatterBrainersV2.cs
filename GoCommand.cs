using System; 

namespace ScatterBrainersV2
{
    public class GoCommand : Command
    {

        public GoCommand() : base()
        {
            this.Name = "go";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.WalkTo(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nGo Where?");
            }
            return false;
        }
    }
  
}