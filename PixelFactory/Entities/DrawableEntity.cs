using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Animations;
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
        public Texture2D Texture { get; set; }
        public Animation Animation { get; set; } = null;
        public Vector2 Position { get => position; set { position = value; drawPosititon = Map.MapToScreen(value.X, value.Y); } }
        public EntityRotation Rotation { get => rotation; set { rotation = value; Rotate(); } }
        public Vector2 Size { get; private set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Zoom { get; set; } = 1f;
        public DrawLayer Layer { get; set; }
        protected Vector2 rotatedSize;
        protected Vector2 drawPosititon;

        private Vector2 position;
        private EntityRotation rotation;


        public DrawableEntity(Vector2 size)
        {
            Size = size;
            rotatedSize = size;

        }
        public DrawableEntity(DrawableEntity entity) : base(entity)
        {
            Texture = entity.Texture;
            Animation = entity.Animation;
            Position = new Vector2(entity.Position.X, entity.position.Y);
            Rotation = entity.Rotation;
            Size = new Vector2(entity.Size.X, entity.Size.Y);
            Scale = new Vector2(entity.Scale.X, entity.Scale.Y);
            Zoom = entity.Zoom;
            Layer= entity.Layer;
            rotatedSize = new Vector2(entity.rotatedSize.X, entity.rotatedSize.Y);
            drawPosititon = new Vector2(entity.drawPosititon.X, entity.drawPosititon.Y);
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
                default:
                    break;
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
                default:
                    break;
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
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layer = GetDrawLayer();
            float rotation = GetRotationAngle();
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 textureSize = new Vector2(Texture.Width, Texture.Height);
            if(Animation != null)
            {
                sourceRectangle = Animation.CurrentFrame;
                textureSize = Animation.FrameSize;
            }
            float scaleWidth = textureSize.X * Scale.X * Zoom;
            float scaleHeight = textureSize.Y * Scale.Y * Zoom;
            spriteBatch.Draw(Texture, new Rectangle((int)((drawPosititon.X * Zoom + (int)scaleWidth / 2)), (int)((drawPosititon.Y * Zoom + (int)scaleHeight / 2)), (int)scaleWidth, (int)scaleHeight), sourceRectangle, Color.White, rotation, new Vector2(textureSize.X / 2, textureSize.Y / 2), SpriteEffects.None, layer);
        }
    }
}
