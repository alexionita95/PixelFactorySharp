using Microsoft.Xna.Framework;
using PixelFactory.Inventory;
using System;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class Port
    {
        public enum PortType
        {
            Input, Output, IO
        }
        public uint Position { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.N;
        public PortType Type { get; set; } = PortType.Input;
        public InventoryEntityType EntityType { get; set; }
        public bool IsInput { get => Type == PortType.Input; }
        public bool IsOutput { get => Type == PortType.Output; }
        public bool IsIO {  get => Type == PortType.IO; }
        public Port(PortType type = PortType.Input, InventoryEntityType entityType = InventoryEntityType.Solid, Direction direction = Direction.N)
        {
            Direction = direction;
            Type = type;
            EntityType = entityType;
        }
    }
}
