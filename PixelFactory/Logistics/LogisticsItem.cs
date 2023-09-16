using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Items;

namespace PixelFactory.Logistics
{
    public class LogisticsItem : DrawableEntity
    {
        public Item Item { get; set; } = null;
        public double Progress { get; set; } = 0;
        public bool Ready { get => Progress.Equals(1); }
        public Direction SourceDirection { get; set; }
        public uint SourcePosition { get; set; }
        public Direction DestinationDirection { get; set; }
        public uint DestinationPosition { get; set; }
        public Vector2 LogisticPosition { get; set; }
        public void Update(double step)
        {
            Progress += step;
            if (Progress > 1)
            {
                Progress = 1;
            }
        }
        public LogisticsItem(SpriteBatch spriteBatch)
            : base(spriteBatch, Vector2.One)
        {
            Layer = DrawLayer.Items;
        }
        public LogisticsItem(Item item, SpriteBatch spriteBatch)
            : base(spriteBatch, Vector2.One)
        {
            Layer = DrawLayer.Items;
            Item = item;
            Progress = 0;
        }
        public override void Draw(GameTime gameTime)
        {
            Texture = ContentManager.Instance.GetItemTexture(Item.Id);
            drawPosititon = LogisticPosition;
            base.Draw(gameTime);
        }
    }
}
