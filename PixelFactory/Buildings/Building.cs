using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PixelFactory.Buildings
{
    public class Building : DrawableEntity
    {
       

        
        protected List<ItemSlot> Inventory;
        protected GameTime lastAction;
        protected float rotation = 0;


        public float ProcessingTime { get; set; }

        public Building(SpriteBatch spriteBatch)
            : base(spriteBatch)
        {
            Inventory = new List<ItemSlot>();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public void AddInventorySlot(ItemSlot slot)
        {
            Inventory.Add(slot);
        }
        protected virtual void Process(GameTime gameTime)
        {
        }
        public override void Update(GameTime gameTime)
        {
            if (lastAction != null)
            {
                if (gameTime.ElapsedGameTime.TotalMilliseconds - lastAction.ElapsedGameTime.TotalMilliseconds > ProcessingTime)
                {
                    lastAction = gameTime;
                    Process(gameTime);
                }
            }
            else
            {
                lastAction = gameTime;
            }
        }
        public virtual bool HasInputInventory()
        {
            int inputCount = 0;
            return inputCount > 0;
        }
        public virtual bool HasOutputInventory()
        {
            int outputCount = 0;
            return outputCount > 0;
        }
        public virtual bool CanAcceptInput(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item == null || !slot.IsFull() && slot.Item.Id == item.Id && (slot.IsInput() || slot.IsIO()) )
                {
                    return true;
                }
            }
            return false;
        }


        public virtual bool CanAcceptOutput(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item == null || !slot.IsFull() && slot.Item.Id == item.Id && (slot.IsOutput() || slot.IsIO()))
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool CanAcceptInputItems()
        {
            foreach (ItemSlot slot in Inventory)
            {
                if ((slot.IsInput() || slot.IsIO()) && !slot.IsFull())
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanExportItems()
        {
            foreach (ItemSlot slot in Inventory)
            {
                if ((slot.IsOutput() || slot.IsIO()) && slot.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool AddInput(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item == null || ((slot.IsInput() || slot.IsIO()) && !slot.IsFull() && slot.Item.Id == item.Id))
                {
                    slot.Item = item;
                    slot.Count++;
                    return true;
                }
            }
            return false;
        }
        public virtual bool AddOutput(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item == null || ((slot.IsOutput() || slot.IsIO()) && !slot.IsFull() && slot.Item.Id == item.Id))
                {
                    slot.Item = item;
                    slot.Count++;
                    return true;
                }
            }
            return false;
        }

        public virtual bool RemoveInput(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item != null && (slot.IsInput() || slot.IsIO()) && slot.Count > 0 && slot.Item.Id == item.Id)
                {
                    slot.Count--;
                    if (slot.Count == 0)
                    {
                        slot.Reset();
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool Remove(Item item)
        {
            foreach (ItemSlot slot in Inventory)
            {
                if (slot.Item != null && (slot.IsOutput() || slot.IsIO()) && slot.Count > 0 && slot.Item.Id == item.Id)
                {
                    slot.Item = item;
                    slot.Count--;
                    if (slot.Count == 0)
                    {
                        slot.Reset();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
