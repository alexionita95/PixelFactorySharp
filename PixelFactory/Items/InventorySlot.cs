using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Transactions;

namespace PixelFactory.Items
{
    public class InventorySlot : Entity
    {
        public enum SlotType { Input, Output, IO }
        public SlotType Type { get; set; } = SlotType.IO;
        //public List<Item> Items { get; set; }
        public InventoryEntity Entity { get; set; }
        public InventoryEntity FilterEntity { get; set; }
        public int Count { get; private set; }
        public bool Filtered { get; set; } = false;
        public bool IsEmpty { get => Count == 0; }
        public int Capacity { get; private set; }
        public int AvailableSpace { get =>  Capacity - Count; }
        public bool IsFull
        {
            get
            {
                if(IsEmpty)
                {
                    return false;
                }
                    return Count == Capacity;
            }
        }

        public InventorySlot(SlotType type = SlotType.IO)
        {
            Type = type;
            Entity = null;
        }
        public InventorySlot(InventoryEntity item, int quantity)
        {
            Entity = new InventoryEntity(item);
            Capacity = Entity.MaximumQuantity;
            AddEntities(item, quantity);
        }

        public InventorySlot(InventoryEntity item, int quantity, int capacity)
        {
            Entity = new InventoryEntity(item);
            Capacity = capacity;
            AddEntities(item, quantity);
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
        public bool HasSameEntity(InventorySlot slot)
        {
            if (slot == null)
            {
                return false;
            }
            return HasSameEntity(slot.Entity);
        }
        public bool HasSameEntity(InventoryEntity item)
        {
            if (Entity == null || item == null)
            {
                return false;
            }

            return Entity.HasSameId(item);
        }
        public bool AddItem(InventoryEntity item)
        {
            AddEntities(item);
            return true;
        }
        public bool CanAccept(InventoryEntity entity, int count = 1)
        {
            if (IsFull)
            {
                return false;
            }
            if (count > entity.MaximumQuantity)
            {
                return false;
            }
            if (Entity == null)
            {
                if (Filtered)
                {
                    if (FilterEntity != null && entity.HasSameId(FilterEntity))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (entity.HasSameId(Entity))
                {
                    if (AvailableSpace >= count)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool AddEntities(InventoryEntity entity, int count = 1)
        {
            if (!CanAccept(entity, count))
            {
                return false;
            }
            if (this.Entity == null)
            {
                this.Entity = new InventoryEntity(entity);
                Capacity = Entity.MaximumQuantity;
            }
            Count += count;
            return true;
        }
        public bool AddEntities(int count = 1)
        {
            return AddEntities(Entity, count);
        }

        public bool CanRemove(InventoryEntity entity, int count = 1)
        {
            if (IsEmpty)
            {
                return false;
            }
            if (Entity == null)
            {
                return false;
            }
            if (!entity.HasSameId(Entity))
            {
                return false;
            }
            int finalCount = Count - count;
            if (finalCount < 0)
            {
                return false;
            }

            return true;
        }
        public bool RemoveEntity(InventoryEntity entity)
        {
            return RemoveEntities(entity);
        }
        public bool RemoveEntity()
        {
            return RemoveEntity(Entity);
        }
        public bool RemoveEntites(int count = 1)
        {
            return RemoveEntities(Entity, count);
        }
        public void RemoveAll()
        {
            Count = 0;
            Reset();
        }
        public void ApplyFilter(InventoryEntity filter)
        {
            FilterEntity = new InventoryEntity(filter);
            Filtered = true;
        }
        public void RemoveFilter()
        {
            Filtered = false;
            FilterEntity = null;
        }
        public bool RemoveEntities(InventoryEntity entity, int count = 1)
        {
            if (!CanRemove(entity, count))
            {
                return false;
            }
            Count -= count;
            Reset();
            return true;
        }
        public bool HasEntities(InventoryEntity entity, int count)
        {
            if(!HasSameEntity(entity))
            {
                return false;
            }
            if(Count == 0)
            {
                return false;
            }
            if(Count < count)
            {
                return false;
            }
            return true;
        }
        public bool HasEntities(int count)
        {
            return HasEntities(Entity, count);
        }
        public void Reset()
        {
            if (Count != 0)
            {
                return;
            }
            if (Entity != null)
            {
                Count = 0;
                Entity = null;
            }
        }
        public override void Update(GameTime gameTime)
        {
            Reset();
            base.Update(gameTime);
        }

    }
}
