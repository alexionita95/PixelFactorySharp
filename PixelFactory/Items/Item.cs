using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class ItemSlot
    {
        public enum SlotType { Input, Output, IO }
        public SlotType Type { get; set; } = SlotType.IO;
        public Item Item { get; set; }
        public int Count { get; set; } = 0;
        public int StackSize { get; set; } = 10;
        public bool Filtered { get; set; } = false;
        public bool IsEmpty { get => (Item == null || Count == 0); }

        public ItemSlot(SlotType type = SlotType.IO)
        {
            Type = type;
        }
        public bool IsInput()
        {
            return Type == SlotType.Input;
        }
        public bool IsOutput()
        {
            return Type == SlotType.Output;
        }
        public bool IsIO()
        {
            return Type == SlotType.IO;
        }
        public bool IsFull()
        {
            return Count == StackSize;
        }
        public void Reset()
        {
            if (!Filtered)
            {
                Item = null;
            }
            Count = 0;
        }
    }
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
