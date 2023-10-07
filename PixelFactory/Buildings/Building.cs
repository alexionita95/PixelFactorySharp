using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
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
        protected List<InventorySlot> Inventory;
        protected GameTime lastAction;
        protected float rotation = 0;

        public float ProcessingTime { get; set; }

        public Building(SpriteBatch spriteBatch, Vector2 size)
            : base(spriteBatch, size)
        {
            Inventory = new List<InventorySlot>();
            Layer = DrawLayer.Buildings;
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pixelSize = Map.MapToScreen(rotatedSize.X, rotatedSize.Y);

            Vector2 newPos = Map.MapToScreen(Position.X, Position.Y);
            float difference;
            switch (Rotation)
            {
                case EntityRotation.None:
                    difference = Texture.Height - pixelSize.Y;
                    newPos.Y -= difference;
                    break;
                case EntityRotation.Rot90:
                    difference = Texture.Height - pixelSize.Y;
                    newPos.Y -= difference / 2;
                    newPos.X += difference / 2;
                    break;
                case EntityRotation.Rot180:
                    break;
                case EntityRotation.Rot270:
                    difference = Texture.Height - pixelSize.Y;
                    newPos.Y -= difference / 2;
                    newPos.X -= difference / 2;
                    break;
            }
            drawPosititon = newPos;
            base.Draw(gameTime);
        }
        public void AddInventorySlot(InventorySlot slot)
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
        public virtual bool CanAcceptInput(InventoryEntity item)
        {
            return false;
        }


        public virtual bool CanAcceptOutput(InventoryEntity item)
        {
            return false;
        }
        public virtual bool CanAcceptInputItems()
        {
            return false;
        }

        public bool CanExportItems()
        {
            foreach (InventorySlot slot in Inventory)
            {
                if ((slot.IsOutput() || slot.IsIO()) && slot.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool AddInput(InventoryEntity item)
        {
            return false;
        }
        public virtual bool AddOutput(InventoryEntity item)
        {
            return false;
        }

        public virtual bool RemoveInput(InventoryEntity item)
        {
            return false;
        }

        public virtual bool Remove(InventoryEntity item)
        { 
            return false;
        }
    }
}
