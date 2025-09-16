using System; 

namespace ScatterBrainersV2
{
	public class UnlockCommand : Command
	{
        public UnlockCommand() : base()
        {
            this.Name = "unlock";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Unlock(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nUnlock What?");
            }
            return false;
        }
    }
}