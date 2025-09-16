using System; 

namespace ScatterBrainersV2
{
    public class QuitCommand : Command
    {
        public QuitCommand() : base()
        {
            this.Name = "quit";
        }

        override
        public bool Execute(Player player)
        {
            bool answer = true;
            if (this.HasSecondWord())
            {
                player.WarningMessage("\nI cannot quit " + this.SecondWord!); // ! tells compiler I know this is not null here
                answer = false;
            }
            return answer;
        }
    }
}