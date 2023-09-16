using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Entities
{



    public class DrawableEntity : Entity
    {
        public enum DrawLayer
        {
            Map, Buildings, Items, UI
        }

        public enum EntityRotation
        {
            None, Rot90, Rot180, Rot270
        }
        public Vector2 Position { get => position; set { position = value; drawPosititon = Map.MapToScreen(value.X, value.Y); } }
        public EntityRotation Rotation { get => rotation; set { rotation = value; Rotate(); } }
        public Vector2 Size { get; private set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public DrawLayer Layer { get; set; }
        protected Vector2 rotatedSize;
        protected Vector2 drawPosititon;
        public Texture2D Texture;
        private Vector2 position;
        private EntityRotation rotation;

        protected SpriteBatch spriteBatch;

        public DrawableEntity(SpriteBatch spriteBatch, Vector2 size)
        {
            this.spriteBatch = spriteBatch;
            Size = size;
            rotatedSize = size;

        }

        public float GetDrawLayer()
        {
            switch (Layer)
            {
                case DrawLayer.Map:
                    return 1;
                case DrawLayer.Buildings:
                    return .5f;
                case DrawLayer.Items:
                case DrawLayer.UI:
                    return 0;
            }
            return 1;
        }
        public float GetRotationAngle()
        {
            switch (Rotation)
            {
                case EntityRotation.None:
                    return MathHelper.ToRadians(0);
                case EntityRotation.Rot90:
                    return MathHelper.ToRadians(90);
                case EntityRotation.Rot180:
                    return MathHelper.ToRadians(180);
                case EntityRotation.Rot270:
                    return MathHelper.ToRadians(270);

            }
            return MathHelper.ToRadians(0);
        }
        private void Rotate()
        {
            switch (Rotation)
            {
                case EntityRotation.None:
                case EntityRotation.Rot180:
                    rotatedSize = Size;
                    break;
                case EntityRotation.Rot90:
                case EntityRotation.Rot270:
                    rotatedSize = new Vector2(Size.Y, Size.X);
                    break;
            }
        }
        public virtual void Draw(GameTime gameTime)
        {
            float layer = GetDrawLayer();
            float rotation = GetRotationAngle();
            float scaleWidth = Texture.Width * Scale.X;
            float scaleHeight = Texture.Height * Scale.Y;
            spriteBatch.Draw(Texture, new Rectangle((int)drawPosititon.X + (int)scaleWidth / 2, (int)drawPosititon.Y + (int)scaleHeight / 2, (int)scaleWidth, (int)scaleHeight), new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White, rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), SpriteEffects.None, layer);
        }
    }
}
