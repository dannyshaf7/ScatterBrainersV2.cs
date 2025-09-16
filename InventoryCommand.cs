using System; 

namespace ScatterBrainersV2
{
	public class InventoryCommand : Command
	{
        public InventoryCommand()
        {
            this.Name = "inventory";
        }

        override
        public bool Execute(Player player)
        {
            player.Inventory();
            return false;
        }
    }
}