using System;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class ItemLogisticsComponentPort
    {
        public enum PortDirection
        {
            N, S, E, W
        }
        public enum PortType
        {
            Input, Output
        }
        public uint Position { get; set; } = 0;
        public PortDirection Direction { get; set; } = PortDirection.N;
        public PortType Type { get; set; } = PortType.Input;
        public List<LogisticsComponentItem> Items { get; set; }

        public bool HasItems { get => Items.Count > 0; }
        public bool IsFull()
        {
            if(Type == PortType.Output && Items.Count == 1) return true;
            return false;
        }


        public ItemLogisticsComponentPort(PortType type = PortType.Input, PortDirection direction = PortDirection.N)
        {
            Direction = direction;
            Type = type;
            Items = new List<LogisticsComponentItem>();
        }

    }
}
