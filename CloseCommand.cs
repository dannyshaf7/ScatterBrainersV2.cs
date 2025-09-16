using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class CloseCommand : Command
    {
        public CloseCommand() : base()
        {
            this.Name = "close";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Close(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nClose What?");
            }
            return false;
        }
    }
}