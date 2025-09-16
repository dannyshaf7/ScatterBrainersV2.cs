using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    public class Item : IItem
    {
        // enscapsulation - private fields are exposed via public properties
        // external code can't directly modify them
        private String _name;
        // Name is read-only once created, can't be changed
        public String Name { get { return _name; } }
        private float _weight;
        // read-only Weight, runs when item.Weight is called to get the value
        // if _decorator is null return zero, otherwise return (and add) decorator weight to Weight
        // calling .Weight on a null object would throw an exception
        public float Weight { get { return _weight + (_decorator == null ? 0 : _decorator.Weight); } }
        private IItem _decorator;
        private float _capacity;
        // Capacity and IsPowerup are read/write, can change at runtime
        public float Capacity { get { return _capacity; } set { _capacity = value; } }
        private bool _powerup;
        public bool IsPowerup { get { return _powerup; } set { _powerup = value; } }
        public String LongName { get { return Name; } }
        public String Description
        {
            get
            {
                return LongName + " - " + Weight;
            }
        }

        public Item() : this("Nameless") {}
        public Item(String name) : this(name, 1f) { } // default item weight is 1f

        // designated constructor
        public Item(String itemName, float weight)
        {
            _name = itemName;
            _weight = weight;
            _decorator = null;
            _capacity = 0.0f;
            _powerup = false;
        }

        // method is recursive - if item doesn't have a decorator, attach the new one
        // if already has decorator, pass new decorator to existing decorator if it 
        // doesn't already have one, otherwise keep passing it along 
        public void AddDecorator(IItem decorator)
        {
            if (_decorator == null)
            {
                _decorator = decorator;
            }
            else
            {
                _decorator.AddDecorator(decorator);
            }
        }
    }
}