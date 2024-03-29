﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class LogisticsEntity : DrawableEntity
    {
        public InventoryEntity Entity { get; set; } = null;
        public float Progress { get; set; } = 0;
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
            Progress += (float)step;
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

        public override List<byte> GetData()
        {
            List<byte> data = base.GetData();
            Serialization.Serializer.WriteString(Entity.Id, data);
            Serialization.Serializer.WriteInt((int)SourceDirection, data);
            Serialization.Serializer.WriteInt((int)SourcePosition, data);
            Serialization.Serializer.WriteInt((int)DestinationDirection, data);
            Serialization.Serializer.WriteInt((int)DestinationPosition, data);
            Serialization.Serializer.WriteFloat(Progress, data);
            return data;
        }
    }
}
