using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class Belt : ItemLogisticsComponent
    {
        public int ItemLimit { get; set; } = 10;
        public bool corner = false;

        public Belt(SpriteBatch spriteBatch) : base(spriteBatch, Vector2.One)
        {
            AddInput(Direction.N, 0);
            AddInput(Direction.E, 0);
            AddInput(Direction.W, 0);
            AddOutput(Direction.S, 0);
        }
        public override void Update(GameTime gameTime)
        {
            int validInputs = GetValidInputsCount();
            if (validInputs == 0)
            {
                corner = false;
            }
            if (validInputs == 1) 
            {
                if (ValidateInput(Direction.E) && ValidateOutput(Direction.S))
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
        private void DrawPortsOnBelt(List<ItemLogisticsComponentPort> ports, GameTime gameTime)
        {
            foreach (var port in ports)
            {
                if (port.HasItems)
                {
                    foreach (var item in port.Items)
                    {
                        DrawItemOnBelt(item, gameTime);
                    }
                }
            }
        }
        private void DrawItemOnBelt(LogisticsItem item, GameTime gameTime)
        {
            var pos = Map.MapToScreen(Position.X, Position.Y);
            item.Zoom = Zoom;
            var itemPos = GetItemPosition(item);
            itemPos = new Vector2(itemPos.X * Map.TileSize, itemPos.Y * Map.TileSize) + pos;
            item.LogisticPosition = itemPos;
            item.Draw(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if(corner)
            {
                Animation.CurrentRow = 1;
            }
            else
            {
                Animation.CurrentRow = 0;
            }
            base.Draw(gameTime);
            DrawPortsOnBelt(Outputs, gameTime);
            DrawPortsOnBelt(Inputs, gameTime);
        }
    }
}
