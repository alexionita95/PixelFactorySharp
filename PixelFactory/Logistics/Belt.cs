using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class Belt : ItemLogisticsComponent
    {
        public int ItemLimit { get; set; } = 10;
        private float itemScale = 0.25f;

        public Belt(SpriteBatch spriteBatch) : base(spriteBatch, Vector2.One)
        {
            AddInput(Direction.N, 0);
            AddInput(Direction.E, 0);
            AddInput(Direction.W, 0);
            AddOutput(Direction.S, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        private void DrawPortsOnBelt(List<ItemLogisticsComponentPort> ports)
        {
            foreach (var port in ports)
            {
                if (port.HasItems)
                {
                    foreach (var item in port.Items)
                    {
                        DrawItemOnBelt(item);
                    }
                }
            }
        }
        private void DrawItemOnBelt(LogisticsItem item)
        {
            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;
            var pos = Map.MapToScreen(Position.X, Position.Y);
            Texture2D itemTexture = ContentManager.Instance.GetItemTexture(item.Item.Id);
            var itemPos = GetItemPosition(item);
            itemPos = new Vector2(itemPos.X * Map.TileSize / 2, itemPos.Y * Map.TileSize / 2);
            spriteBatch.Draw(itemTexture, new Rectangle((int)(pos.X + itemPos.X - scaleWidth / 2), (int)(pos.Y + itemPos.Y - scaleHeight / 2), (int)scaleWidth, (int)scaleHeight),null, Color.White,0f,Vector2.Zero,SpriteEffects.None,0f);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            DrawPortsOnBelt(Outputs);
            DrawPortsOnBelt(Inputs);
        }
    }
}
