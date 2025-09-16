using System; 

namespace ScatterBrainersV2
{
	public class LockCommand : Command
	{
		public LockCommand() : base()
        {
            this.Name = "lock";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Lock(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nLock What?");
            }
            return false;
        }
    }
}