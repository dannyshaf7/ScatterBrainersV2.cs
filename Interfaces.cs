using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public interface IItem
    {
        String Name { get; }
        float Weight { get; }
        float Capacity { get; set; }
        String LongName { get; }
        String Description { get; }
        bool IsPowerup { get; }
        void AddDecorator(IItem decorator);
    }
    public interface IItemContainer : IItem
    {
        bool Add(IItem item);
        IItem Get(string name);
    }
    public interface IClosable
    {
        bool IsOpen { get; }
        bool IsClosed { get; }
        bool Open();
        bool Close();
    }
    public interface ILockable : IClosable
    {
        bool IsLocked { get; }
        bool IsUnlocked { get; }
        bool Lock();
        bool Unlock();
        bool OnOpen();
        bool OnClose();
    }
    public interface ICharacter
    {
        Container PlayerContainer { get; set; }
        Room CurrentRoom { get; set; }
        void WalkTo(String direction);
        void Defend();
        void TakeItem(String itemName);
        bool Open(String name);
    }
}