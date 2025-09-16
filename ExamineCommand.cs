using System; 

namespace ScatterBrainersV2
{
    public class ExamineCommand : Command
    {
        public ExamineCommand() : base()
        {
            this.Name = "examine";
        }

        override
        public bool Execute(Player player)
        {
            if (this.HasSecondWord())
            {
                player.Examine(this.SecondWord!); // ! tells compiler I know this is not null here
            }
            else
            {
                player.WarningMessage("\nExamine what?");
            }
            return false;
        }
    }
}