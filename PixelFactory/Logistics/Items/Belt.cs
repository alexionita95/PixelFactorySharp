using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Inventory;
using System.Collections.Generic;

namespace PixelFactory.Logistics.Items
{
    public class Belt : ItemLogisticsComponent
    {
        public bool corner = false;
        public Belt()
        {
            AddInput(Direction.N, 0);
            AddInput(Direction.E, 0);
            AddInput(Direction.W, 0);
            AddOutput(Direction.S, 0);
            CurrentOutputIndex = 3;
        }
        public override void Update(GameTime gameTime)
        {
            InventoryEntityType type = InventoryEntityType.Solid;
            int validInputs = GetValidInputsCount(type);
            if (validInputs == 0)
            {
                corner = false;
            }
            if (validInputs == 1)
            {
                if (ValidateInput(type, Direction.E) && ValidateOutput(type, Direction.S))
                {
                    corner = true;
                }
            }
            if (validInputs > 1)
            {
                corner = false;
            }
            base.Update(gameTime);
        }
        private void DrawItemOnBelt(LogisticsEntity item, GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pos = Map.MapToScreen(Position.X, Position.Y);
            item.Zoom = Zoom;
            var itemPos = GetItemPosition(item);
            itemPos = new Vector2(itemPos.X * Map.TileSize, itemPos.Y * Map.TileSize) + pos;
            item.LogisticPosition = itemPos;
            item.Draw(gameTime, spriteBatch);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture.Rotation= Rotation;
            if (corner)
            {
                Animation.CurrentRow = 1;
            }
            else
            {
                Animation.CurrentRow = 0;
            }
            base.Draw(gameTime, spriteBatch);

            foreach (var item in Items)
            {
                DrawItemOnBelt(item, gameTime, spriteBatch);
            }
            //DrawPortsOnBelt(Ports, gameTime, spriteBatch);
            // DrawPortsOnBelt(Inputs, gameTime, spriteBatch);
        }
    }
}
