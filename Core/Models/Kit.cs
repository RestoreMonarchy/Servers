using System.Collections.Generic;

namespace Core.Models
{
    public class Kit
    {
        public Kit() { }
        public Kit(string name, int cooldown, List<Item> items, uint experience = 0, ushort vehicle = 0, decimal price = 0)
        {
            Name = name;
            Cooldown = cooldown;
            Experience = experience;
            Vehicle = vehicle;
            Price = price;
            Items = items;
        }

        public string Name { get; set; }
        public int Cooldown { get; set; }
        public uint Experience { get; set; }
        public ushort Vehicle { get; set; }
        public decimal Price { get; set; }

        public List<Item> Items { get; set; }
        
        public class Item
        {
            public Item() { }
            public Item(ushort itemId, byte[] state = null)
            {
                ItemId = itemId;
                State = state;
            }

            public ushort ItemId { get; set; }
            public byte[] State { get; set; }
        }
    }
}
