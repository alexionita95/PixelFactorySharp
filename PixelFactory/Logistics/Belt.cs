using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Logistics
{
    public class Belt : ItemLogisticsComponent
    {

        public enum BeltType
        {
            I, L, T
        }

        BeltType Type { get; set; } = BeltType.I;

        public int ItemLimit { get; set; } = 10;


        private float itemScale = 0.25f;
        private float itemOverlap = 1;


        public Belt(SpriteBatch spriteBatch) : base(spriteBatch, Vector2.One)
        {
            AddInput(ItemLogisticsComponentPort.PortDirection.N, 0);
            AddInput(ItemLogisticsComponentPort.PortDirection.E, 0);
            AddInput(ItemLogisticsComponentPort.PortDirection.W, 0);
            AddOutput(ItemLogisticsComponentPort.PortDirection.S, 0);
            // AddInput(ItemLogisticsComponentPort.PortDirection.E, 0);
            //AddInput(ItemLogisticsComponentPort.PortDirection.W, 0);
        }

        public Vector2 GetItemPos(double progress, Vector2 origin)
        {
            return Vector2.Zero;
        }

        private bool ValidateDistance(Vector2 dist)
        {
            Vector2 origin = new Vector2(Map.TileSize / 2, Map.TileSize / 2);
            return dist.Length() > itemOverlap * itemScale * Map.TileSize;
        }
      




        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void DrawItemOnBelt(LogisticsItem item)
        {
            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;
            var pos = Map.MapToScreen(Position.X, Position.Y);
            Texture2D itemTexture = ContentManager.Instance.GetItemTexture(item.Item.Id);
            var itemPos = GetItemPosition(item);
            itemPos = new Vector2(itemPos.X * Map.TileSize / 2, itemPos.Y * Map.TileSize / 2);
            spriteBatch.Draw(itemTexture, new Rectangle((int)(pos.X + itemPos.X - scaleWidth / 2), (int)(pos.Y + itemPos.Y - scaleHeight / 2), (int)scaleWidth, (int)scaleHeight), Color.White);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;
            var pos = Map.MapToScreen(Position.X, Position.Y);
            
            foreach(var output in Outputs)
            {
                if(output.HasItems)
                {
                    foreach (var item in output.Items)
                    {
                        DrawItemOnBelt(item);
                    }
                }
            }
            foreach(var input in Inputs)
            {
                if (input.HasItems)
                {
                    foreach (var item in input.Items)
                    {
                        DrawItemOnBelt(item);
                    }
                }
                
            }
            
            

        }

    }
}
