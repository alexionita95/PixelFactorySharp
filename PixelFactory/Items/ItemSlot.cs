using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System.Collections.Generic;

namespace PixelFactory.Items
{
    public class ItemSlot : Entity
    {
        public enum SlotType { Input, Output, IO }
        public SlotType Type { get; set; } = SlotType.IO;
        public List<Item> Items { get; set; }
        public Item FilterItem { get; set; }
        public int Count { get => Items.Count; }
        public bool Filtered { get; set; } = false;
        public bool IsEmpty { get => (Items.Count == 0); }
        public bool IsFull
        {
            get
            {
                if (FilterItem != null)
                    return Items.Count == FilterItem.StackSize;
                return false;
            }
        }

        public ItemSlot(SlotType type = SlotType.IO)
        {
            Type = type;
            Items = new List<Item>();
        }
        public ItemSlot(Item item, int quantity)
        {
            Items = new List<Item>();
            AddItems(item, quantity);
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

        public void AddItem(Item item)
        {
            if (FilterItem == null)
                FilterItem = item;
            if (FilterItem != item)
                return;
            Items.Add(item);
        }
        public void AddItems(Item item, int count)
        {
            if(Count + count > item.StackSize) 
                return;
            if (FilterItem == null)
                FilterItem = item;
            if (FilterItem != item)
                return;
            for (int i = 0; i < count; i++)
            {
                Items.Add(new Item(item));
            }
        }
        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
        public void RemoveItem()
        {
            Items.RemoveAt(Count - 1);
        }
        public bool RemoveItems(int count)
        {
            if (count > Count)
                return false;
            Items.RemoveRange(Items.Count - count, count);
            return true;
        }
        public void Reset()
        {
            if (!Filtered && FilterItem != null)
            {
                FilterItem = null;
                Items.Clear();
            }
            if(Filtered)
            {
                Items.Clear();
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Count == 0)
            {
                Reset();
            }
            base.Update(gameTime);
        }

    }
}
