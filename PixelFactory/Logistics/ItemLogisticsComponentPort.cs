using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class ItemLogisticsComponentPort
    {
       
        public enum PortType
        {
            Input, Output
        }
        public uint Position { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.N;
        public PortType Type { get; set; } = PortType.Input;
        public List<LogisticsItem> Items { get; set; }
        public bool Empty { get => Items.Count == 0; }
        public bool HasItems { get => Items.Count > 0; }
        public bool IsFull()
        {
            if (Type == PortType.Output && Items.Count == 1) return true;
            return false;
        }
        public ItemLogisticsComponentPort(PortType type = PortType.Input, Direction direction = Direction.N)
        {
            Direction = direction;
            Type = type;
            Items = new List<LogisticsItem>();
        }
    }
}
