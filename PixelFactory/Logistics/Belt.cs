using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelFactory.Items;
using System.Diagnostics;

namespace PixelFactory.Logistics
{
    public class BeltItem
    {
        public Item Item { get; set; } = null;
        public double Progress { get; set; } = 0;
        public bool Ready { get => Progress.Equals(1); }

        public void Update(double step)
        {
            Progress += step;
            if (Progress > 1)
            {
                Progress = 1;
            }
        }
        public BeltItem()
        {
        }
        public BeltItem(Item item)
        {
            Item = item;
        }

    }
    public class Belt : DrawableEntity
    {


        public enum BeltDirection
        {
            NS, SN, EW, WE, NW, WN, NE, EN, SW, WS, SE, ES
        }
        public enum BeltType
        {
            Straight, Corner
        }
        BeltDirection Direction { get; set; } = BeltDirection.NS;
        BeltType Type { get; set; } = BeltType.Straight;
        public float ProcessingTime { get; set; }
        public int ItemLimit { get; set; } = 10;
        private List<BeltItem> Input { get; set; }
        private List<Item> Output { get; set; }

        private float itemScale = 0.25f;
        private float itemOverlap = 1;
        public Belt(SpriteBatch spriteBatch)
       : base(spriteBatch)
        {
            Input = new List<BeltItem>();
            Output = new List<Item>();

        }
        public bool CanAcceptItems()
        {
            bool result = true;
            if (Input.Count > 0)
            {
                var item = Input.Last();
                if (item != null)
                {
                    Vector2 origin = new Vector2(Map.TileSize / 2, Map.TileSize / 2);
                    Vector2 pos = GetItemPos(item.Progress, origin);
                    Vector2 startPos = GetItemPos(0, origin);
                    result = ValidateDistance(pos - startPos);
                }
            }
            return result && Input.Count + Output.Count < ItemLimit;
        }
        public bool AddItem(Item item)
        {
            if (!CanAcceptItems()) return false;

            BeltItem beltItem = new BeltItem(item);
            Input.Add(beltItem);
            return true;
        }

        private void Rotate()
        {
            switch (Direction)
            {
                case BeltDirection.NS:
                    Type = BeltType.Straight;
                    Rotation = EntityRotation.None;
                    break;
                case BeltDirection.SN:
                    Type = BeltType.Straight;
                    Rotation = EntityRotation.Rot180;
                    break;
                case BeltDirection.EW:
                    Type = BeltType.Straight;
                    Rotation = EntityRotation.Rot270;
                    break;
                case BeltDirection.WE:
                    Type = BeltType.Straight;
                    Rotation = EntityRotation.Rot90;
                    break;
                case BeltDirection.NW:
                    break;
                case BeltDirection.WN:
                    break;
                case BeltDirection.NE:
                    break;
                case BeltDirection.EN:
                    break;
                case BeltDirection.SW:
                    break;
                case BeltDirection.WS:
                    break;
                case BeltDirection.SE:
                    break;
                case BeltDirection.ES:
                    break;
            }
        }
        public Vector2 GetItemPos(double progress, Vector2 origin)
        {

            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;
            float yPos;
            float xPos;
            float width = Map.TileSize;
            float height = Map.TileSize;
            switch (Direction)
            {

                case BeltDirection.NS:
                    yPos = (float)progress * height - scaleHeight / 2;
                    return new Vector2(origin.X - scaleWidth / 2, yPos);
                case BeltDirection.SN:
                    yPos = Map.TileSize - (float)progress * height - scaleHeight / 2;
                    return new Vector2(origin.X - scaleWidth / 2, yPos);
                case BeltDirection.EW:
                    xPos = (float)progress * width - scaleWidth / 2;
                    return new Vector2(xPos, origin.Y - scaleHeight / 2);
                case BeltDirection.WE:
                    xPos = Map.TileSize - (float)progress * width - scaleWidth / 2;
                    return new Vector2(xPos, origin.Y - scaleHeight / 2);
                case BeltDirection.NW:
                    break;
                case BeltDirection.WN:
                    break;
                case BeltDirection.NE:
                    break;
                case BeltDirection.EN:
                    break;
                case BeltDirection.SW:
                    break;
                case BeltDirection.WS:
                    break;
                case BeltDirection.SE:
                    break;
                case BeltDirection.ES:
                    break;
            }
            return Vector2.Zero;
        }
        private bool ValidateDistance(Vector2 dist)
        {
            Vector2 origin = new Vector2(Map.TileSize / 2, Map.TileSize / 2);
            return dist.Length() > itemOverlap * itemScale * Map.TileSize;
        }
        private bool CanMove(BeltItem item)
        {
            int index = Input.IndexOf(item);
            Vector2 origin = new Vector2(Map.TileSize / 2, Map.TileSize / 2);
            Vector2 beforePos;
            Vector2 pos = GetItemPos(item.Progress, origin);
            if (index != -1)
            {
                int before = index - 1;
                if (before < 0)
                {
                    if (Output.Count != 0)
                    {
                        beforePos = GetItemPos(1, origin);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    BeltItem beforeItem = Input[before];
                    beforePos = GetItemPos(beforeItem.Progress, origin);
                }
                if (ValidateDistance(pos - beforePos))
                {
                    return true;
                }
            }
            /* switch (Direction)
             {

                 case BeltDirection.NS:
                 case BeltDirection.SN:
                     {
                         if (size.Y >= Map.TileHeight)
                             return false;
                     }
                     break;
                 case BeltDirection.EW:
                 case BeltDirection.WE:
                     {
                         if (size.X >= Map.TileWidth)
                             return false;
                     }
                     break;
                 case BeltDirection.NW:
                     break;
                 case BeltDirection.WN:
                     break;
                 case BeltDirection.NE:
                     break;
                 case BeltDirection.EN:
                     break;
                 case BeltDirection.SW:
                     break;
                 case BeltDirection.WS:
                     break;
                 case BeltDirection.SE:
                     break;
                 case BeltDirection.ES:
                     break;
             }*/
            return false;
        }
        public override void Update(GameTime gameTime)
        {
            List<BeltItem> toRemove = new List<BeltItem>();
            for (int index = 0; index < Input.Count; ++index)
            {
                var item = Input[index];
                double progress = gameTime.ElapsedGameTime.TotalMilliseconds / ProcessingTime;
                if (CanMove(item) && !item.Ready && Output.Count == 0)
                {
                    item.Update(progress);
                }

                if (item.Ready && Output.Count == 0)
                {
                    toRemove.Add(item);
                    Output.Add(item.Item);
                }
            }
            foreach (var item in toRemove)
            {
                Input.Remove(item);
            }
            base.Update(gameTime);
        }

        private void DrawItemOnBelt(double progress, string itemId)
        {
            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;
            var pos = Map.MapToScreen(Position.X, Position.Y);
            Texture2D itemTexture = ContentManager.Instance.GetItemTexture(itemId);
            var itemPos = GetItemPos(progress, new Vector2(Map.TileSize / 2, Map.TileSize / 2));
            spriteBatch.Draw(itemTexture, new Rectangle((int)(pos.X + itemPos.X), (int)(pos.Y + itemPos.Y), (int)scaleWidth, (int)scaleHeight), Color.White);
        }
        public override void Draw(GameTime gameTime)
        {
            Rotate();
            base.Draw(gameTime);
            var pos = Map.MapToScreen(Position.X, Position.Y);
            float scaleWidth = itemScale * Map.TileSize;
            float scaleHeight = itemScale * Map.TileSize;

            for (int index = 0; index < Output.Count; ++index)
            {
                var item = Output[index];
                DrawItemOnBelt(1, item.Id);
            }

            for (int index = 0; index < Input.Count; ++index)
            {
                var item = Input[index];
                DrawItemOnBelt(item.Progress, item.Item.Id);
            }

        }
    }
}
