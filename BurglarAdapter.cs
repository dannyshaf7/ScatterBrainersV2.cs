using System;
namespace ScatterBrainersV2
{
    public class BurglarAdapter : ICharacter
    {
        Burglar theBurglar;

        public BurglarAdapter(Burglar newBurglar)
        {
            theBurglar = newBurglar;
        }

        public Container PlayerContainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Room CurrentRoom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Defend()
        {
            theBurglar.Attack();
        }

        public bool Open(string name)
        {
            theBurglar.Pillage(name);
            return true;
        }

        public void TakeItem(string itemName)
        {
            theBurglar.StealItem(itemName);
        }

        public void WalkTo(string direction)
        {
            theBurglar.SearchHome(direction);
        }
    }
}