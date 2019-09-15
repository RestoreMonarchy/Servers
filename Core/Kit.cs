using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Core
{
    public class Kit
    {
        public Kit() { }
        public Kit(string name, int cooldown, List<Item> items, uint experience = 0, ushort? vehicle = null, decimal price = 0)
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
        public ushort? Vehicle { get; set; }
        public decimal Price { get; set; }

        public List<Item> Items { get; set; }

        public class Item
        {
            public Item() { }
            public Item(ushort itemId, byte amount)
            {
                ItemId = itemId;
                Amount = amount;
            }

            [XmlArrayAttribute("id")]
            public ushort ItemId { get; set; }
            [XmlArrayAttribute("amount")]
            public byte Amount { get; set; }
        }
    }
}
