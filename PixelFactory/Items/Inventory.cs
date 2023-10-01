using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelFactory.Items
{
    public class Inventory : Entity
    {
        public List<ItemSlot> Slots { get; private set; }
        public ulong Size { get; private set; } = 10;
        public bool HasEmptySlot
        {
            get
            {
                foreach (ItemSlot slot in Slots)
                    if (slot.IsEmpty)
                        return true;
                return false;
            }
        }

        public bool IsFull
        {
            get
            {
                foreach (ItemSlot slot in Slots)
                    if (!slot.IsFull)
                        return false;
                return true;
            }
        }
        public Inventory()
        {
            Slots = new List<ItemSlot>();
            Initialize();
        }
        public Inventory(ulong size)
        {
            Slots = new List<ItemSlot>();
            Size = size;
            Initialize();
        }
        private void Initialize()
        {
            for (ulong i = 0; i < Size; ++i)
            {
                Slots.Add(new ItemSlot());
            }
        }

        public void AddItem(Item item)
        {
            if (IsFull)
            {
                return;
            }
            bool added = false;
            foreach (ItemSlot slot in Slots)
            {
                if (!slot.IsFull && !slot.IsEmpty && slot.FilterItem.Id == item.Id)
                {
                    slot.AddItem(item);
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                foreach (ItemSlot slot in Slots)
                {
                    if (slot.IsEmpty)
                    {
                        slot.AddItem(item);
                        break;
                    }
                }
            }
            Sort();
        }
        public void Sort()
        {
            Slots = Slots.OrderByDescending(slot => slot.FilterItem?.Id)
                    .ThenByDescending(slot => slot.Count).ToList();
        }
        public void AddItems(Item item, int quantity)
        {
            if (IsFull)
                return;
            for (int i = 0; i < quantity; ++i)
            {
                AddItem(item);
            }
        }
        public void RemoveItem(Item item)
        {
            foreach (ItemSlot slot in Slots)
            {
                if (!slot.IsEmpty && slot.FilterItem.Id == item.Id)
                {
                    slot.RemoveItem();
                    return;
                }
            }
        }
        public void RemoveItem(Item item, int quantity)
        {
            int toRemove = quantity;
            for (int i = Slots.Count - 1; i >= 0; --i)
            {
                ItemSlot slot = Slots[i];
                if (!slot.IsEmpty && slot.FilterItem.Id == item.Id)
                {
                    if (slot.Count < toRemove)
                    {
                        toRemove -= slot.Count;
                        slot.RemoveItems(slot.Count);
                    }
                    else
                    {
                        slot.RemoveItems(toRemove);
                        toRemove -= toRemove;
                    }
                    if (toRemove == 0)
                        return;
                }
            }
        }
        public bool HasItems(Item item, int quantity)
        {
            int inventoryQuantity = 0;
            foreach (ItemSlot slot in Slots)
            {
                if (!slot.IsEmpty && slot.FilterItem.Id == item.Id)
                    inventoryQuantity += slot.Count;
            }
            return inventoryQuantity >= quantity;

        }
        public bool HasEmptySlots(int count)
        {
            int result = 0;
            foreach (ItemSlot slot in Slots)
            {
                if (slot.IsEmpty)
                    result += 1;
            }
            return result >= count;
        }
        public bool CanAccept(Item item, int quantity = 1)
        {
            int acceptQuantity = 0;
            foreach (ItemSlot slot in Slots)
            {
                if (!slot.IsEmpty && !slot.IsFull && slot.FilterItem.Id != item.Id)
                    acceptQuantity += (slot.FilterItem.StackSize - slot.Count);
            }
            if (acceptQuantity >= quantity)
            {
                return true;
            }
            double neededQuantity = quantity - acceptQuantity;
            int neededSlots = (int)Math.Ceiling(neededQuantity / item.StackSize);
            if (HasEmptySlots(neededSlots))
            {
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (ItemSlot slot in Slots)
            {
                slot.Update(gameTime);
            }
            base.Update(gameTime);
        }
    }
}
