using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class DrawableEntity : Entity
    {
        public enum EntityRotation
        {
            None, Rot90, Rot180, Rot270
        }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; private set; }

        protected Vector2 rotatedSize;
        public EntityRotation Rotation { get; private set; } = EntityRotation.None;
        public Texture2D Texture;
        protected SpriteBatch spriteBatch;

        public DrawableEntity(SpriteBatch spriteBatch, Vector2 size)
        {
            this.spriteBatch = spriteBatch;
            Size = size;
            rotatedSize = size;
            
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
        public void Rotate(EntityRotation rotation)
        {
            Rotation = rotation;
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

            Texture = ContentManager.Instance.GetBuildingTexture(Id);
            Vector2 pixelSize = Map.MapToScreen(rotatedSize.X, rotatedSize.Y);

            Vector2 newPos = Map.MapToScreen(Position.X, Position.Y);
            // newPos.Y -= difference;
            float rotation = GetRotationAngle();
            float difference = 0;
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
            spriteBatch.Draw(Texture, new Rectangle((int)newPos.X + Texture.Width / 2, (int)newPos.Y + Texture.Height / 2, Texture.Width, Texture.Height), new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White, rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), SpriteEffects.None, 0);
        }
    }
}
