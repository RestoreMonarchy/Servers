using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Core
{
    public class Kit
    {
        public Kit() { }
        public Kit(string name, int cooldown, List<ushort> items, List<MetadataItem> metadataItems, uint experience = 0, ushort vehicle = 0, decimal price = 0)
        {
            Name = name;
            Cooldown = cooldown;
            Experience = experience;
            Vehicle = vehicle;
            Price = price;
            Items = items;
            MetadataItems = metadataItems;
        }

        public string Name { get; set; }
        public int Cooldown { get; set; }
        public uint Experience { get; set; }
        public ushort Vehicle { get; set; }
        public decimal Price { get; set; }

        [XmlArrayItem("itemid")]
        public List<ushort> Items { get; set; }
        public List<MetadataItem> MetadataItems { get; set; }
        
        public class MetadataItem
        {
            public MetadataItem() { }
            public MetadataItem(ushort itemId, byte[] metadata)
            {
                ItemId = itemId;
                Metadata = metadata;
            }

            [XmlAttribute("id")]
            public ushort ItemId { get; set; }
            public byte[] Metadata { get; set; }
        }
    }
}
