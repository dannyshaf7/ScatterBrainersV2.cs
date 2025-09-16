using System; 

namespace ScatterBrainersV2
{
    public class PlaceCommand : Command
    {
        public PlaceCommand() : base()
        {
            this.Name = "place";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.PlaceItem(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nPlace What?");
            }
            return false;
        }
    }
}