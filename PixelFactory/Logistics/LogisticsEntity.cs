using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Inventory;

namespace PixelFactory.Logistics
{
    public class LogisticsEntity : DrawableEntity
    {
        public InventoryEntity Entity { get; set; } = null;
        public double Progress { get; set; } = 0;
        public bool Ready { get => Progress.Equals(1); }
        public Direction SourceDirection { get; set; }
        public uint SourcePosition { get; set; }
        public Direction DestinationDirection { get; set; }
        public uint DestinationPosition { get; set; }
        public Vector2 LogisticPosition { get; set; }
        public bool ReachedDestination { get; private set; } = false;
        public void Update(double step)
        {
            if (ReachedDestination)
                return;
            Progress += step;
            if (Progress > 1)
            {
                Progress = 1;
                ReachedDestination = true;
            }
        }
        public LogisticsEntity()
            : base(Vector2.One)
        {
            Layer = DrawLayer.Items;
        }
        public LogisticsEntity(InventoryEntity item)
            : base(Vector2.One)
        {
            Layer = DrawLayer.Items;
            Entity = item;
            Progress = 0;
            ReachedDestination = false;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture = Entity.Texture;
            drawPosititon = LogisticPosition;
            base.Draw(gameTime, spriteBatch);
        }
    }
}
