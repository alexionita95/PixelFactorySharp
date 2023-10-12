using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelFactory.Inventory
{
    public class Inventory : Entity
    {
        public List<InventorySlot> Slots { get; private set; }
        public ulong Size { get; private set; } = 10;
        public bool AutoSort {  get; set; } = true;
        bool changed = false;
        public bool HasEmptySlot
        {
            get
            {
                foreach (InventorySlot slot in Slots)
                    if (slot.IsEmpty)
                        return true;
                return false;
            }
        }

        public bool IsFull
        {
            get
            {
                foreach (InventorySlot slot in Slots)
                    if (!slot.IsFull)
                        return false;
                return true;
            }
        }
        public Inventory()
        {
            Slots = new List<InventorySlot>();
            Initialize();
        }
        public Inventory(ulong size)
        {
            Slots = new List<InventorySlot>();
            Size = size;
            Initialize();
        }
        private void Initialize()
        {
            for (ulong i = 0; i < Size; ++i)
            {
                Slots.Add(new InventorySlot());
            }
        }
        public void AddEntity(InventoryEntity entity)
        {
            AddEntities(entity);
        }
        public void AddEntities(InventoryEntity entity, float count = 1)
        {
            if (IsFull)
            {
                return;
            }
            if (!CanAccept(entity, count))
            {
                return;
            }
            float neededQuantity = count;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.IsFull)
                {
                    continue;
                }

                float addCount = entity.MaximumQuantity;
                if (!slot.IsEmpty)
                {
                    addCount = slot.AvailableSpace;
                }
                if (addCount > neededQuantity)
                {
                    addCount = neededQuantity;
                }
                if (slot.AddEntities(entity, addCount))
                {
                    neededQuantity -= addCount;
                    if (neededQuantity == 0)
                    {
                        changed= true;
                        Ballance();
                        return;
                    }
                }
            }
        }
        public void Ballance()
        {
            if(!AutoSort)
            {
                return;
            }
            Sort();
            for (int i = 0; i < Slots.Count; ++i)
            {
                InventorySlot currentSlot = Slots[i];
                if (!currentSlot.IsFull)
                {
                    for (int j = i + 1; j < Slots.Count; ++j)
                    {
                        InventorySlot slot = Slots[j];
                        if (slot.HasSameEntity(currentSlot))
                        {
                            float neededCount = currentSlot.Capacity - currentSlot.Count;
                            if (neededCount > 0)
                            {
                                if (slot.Count >= neededCount)
                                {
                                    slot.RemoveEntites(neededCount);
                                    currentSlot.AddEntities(neededCount);
                                }
                                else
                                {
                                    currentSlot.AddEntities(slot.Count);
                                    slot.RemoveAll();
                                }
                            }
                        }
                    }
                }
            }
            Sort();
        }
        public void Sort()
        {
            Slots = Slots.OrderByDescending(slot => slot.Entity?.Id)
                    .ThenByDescending(slot => slot.Count).ToList();
        }
        public void RemoveEntities(InventoryEntity entity, float count = 1)
        {
            if(!HasEntities(entity, count))
            { 
                return;
            }
            float neededCount = count;
            foreach (InventorySlot slot in Slots)
            {
                float currentCount = neededCount;
                if(currentCount > slot.Count)
                {
                    currentCount = slot.Count;
                }
                if (slot.RemoveEntities(entity, currentCount))
                {
                    neededCount -= currentCount;
                    if(neededCount == 0)
                    {
                        Ballance();
                        changed = true;
                        return;
                    }
                }

            }
        }
        public void RemoveEntity(InventoryEntity entity)
        {
            RemoveEntities(entity);
        }
        public bool HasEntities(InventoryEntity entity, float quantity)
        {
            float inventoryQuantity = 0;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.HasSameEntity(entity))
                {
                    if (slot.HasEntities(entity, quantity))
                    {
                        return true;
                    }
                    inventoryQuantity += slot.Count;
                    if (inventoryQuantity > quantity)
                    {
                        return true;
                    }
                }
            }
            return inventoryQuantity >= quantity;

        }
        public bool HasEmptySlots(int count = 1)
        {
            int result = 0;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.IsEmpty)
                    result += 1;
            }
            return result >= count;
        }
        public bool CanAccept(InventoryEntity entity, float quantity = 1)
        {
            float acceptQuantity = 0;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.HasSameEntity(entity))
                {
                    if (slot.CanAccept(entity, quantity))
                    {
                        return true;
                    }
                    acceptQuantity += slot.AvailableSpace;
                    if (acceptQuantity >= quantity)
                    {
                        return true;
                    }
                }
            }
            double neededQuantity = quantity - acceptQuantity;
            int neededSlots = (int)Math.Ceiling(neededQuantity / entity.MaximumQuantity);
            if (HasEmptySlots(neededSlots))
            {
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (InventorySlot slot in Slots)
            {
                slot.Update(gameTime);
            }
            if (changed)
            {
                changed = false;
                Ballance();
            }
            base.Update(gameTime);
        }
        public override List<byte> GetData()
        {
            List<byte> data = base.GetData();
            Serialization.Serializer.WriteInt(Slots.Count, data);
            for(int i=0; i< Slots.Count; ++i)
            {
                data.AddRange(Slots[i].GetData());
            }
            return data;
        }
    }
}
