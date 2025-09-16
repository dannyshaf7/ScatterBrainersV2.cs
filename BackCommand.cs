using System;

namespace ScatterBrainersV2
{
    public class BackCommand : Command
    {
        public BackCommand()
        {
            this.Name = "back";
        }

        public override bool Execute(Player player)
        {
            player.Back();
            return false;
        }
    }
}