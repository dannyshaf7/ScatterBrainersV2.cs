using System; 

namespace ScatterBrainersV2
{
    public class OpenCommand : Command
    {
        public OpenCommand() : base()
        {
            this.Name = "open";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Open(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nOpen What?");
            }
            return false;
        }
    }
}