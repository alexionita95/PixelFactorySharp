using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.UI
{
    public class UIControl
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 AbsolutePosition { get => Parent == null ? Position : Parent.AbsolutePosition + Position; }
        public UIControl Parent { get; set; } = null;
        public List<UIControl> Controls { get; set; }
        public Texture2D Texture { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public int PatchSize { get; set; } = 16;
        public bool TrasnparentBackground { get; set; } = false;

        Rectangle topLeftSrc;
        Rectangle topCenterSrc;
        Rectangle topRightSrc;
        Rectangle middleLeftSrc;
        Rectangle middleCenterSrc;
        Rectangle middleRightSrc;
        Rectangle bottomLeftSrc;
        Rectangle bottomCenterSrc;
        Rectangle bottomRightSrc;

        public UIControl()
        {
            Initialize();
        }
        public UIControl(Vector2 position, Vector2 size)
        {
            Initialize();
            Position = position;
            Size = size;
        }
        public void AddControl(UIControl control)
        {
            control.Parent = this;
            control.SpriteBatch = SpriteBatch;
            Controls.Add(control);
        }
        public void ClearControls()
        {
            Controls.Clear();
        }

        private void Initialize()
        {
            Controls = new List<UIControl>();
            topLeftSrc = new Rectangle(0, 0, PatchSize, PatchSize);
            topCenterSrc = new Rectangle(PatchSize, 0, PatchSize, PatchSize);
            topRightSrc = new Rectangle(2 * PatchSize, 0, PatchSize, PatchSize);
            middleLeftSrc = new Rectangle(0, PatchSize, PatchSize, PatchSize);
            middleCenterSrc = new Rectangle(PatchSize, PatchSize, PatchSize, PatchSize);
            middleRightSrc = new Rectangle(2 * PatchSize, PatchSize, PatchSize, PatchSize);
            bottomLeftSrc = new Rectangle(0, 2 * PatchSize, PatchSize, PatchSize);
            bottomCenterSrc = new Rectangle(PatchSize, 2 * PatchSize, PatchSize, PatchSize);
            bottomRightSrc = new Rectangle(2 * PatchSize, 2 * PatchSize, PatchSize, PatchSize);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var control in Controls)
            {
                control.Update(gameTime);
            }
        }
        private void DrawBackground()
        {
            int layer = 0;
            Rectangle destination = new Rectangle(AbsolutePosition.ToPoint(), new Point(PatchSize, PatchSize));
            SpriteBatch.Draw(Texture, destination, topLeftSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X += PatchSize;
            destination.Width = (int)Size.X - 2 * PatchSize;
            SpriteBatch.Draw(Texture, destination, topCenterSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X += destination.Width;
            destination.Width = PatchSize;
            SpriteBatch.Draw(Texture, destination, topRightSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X = (int)AbsolutePosition.X;
            destination.Y += PatchSize;
            destination.Width = (int)PatchSize;
            destination.Height = (int)Size.Y - 2 * PatchSize;
            SpriteBatch.Draw(Texture, destination, middleLeftSrc, Color.White, 0, new Vector2(0,0), SpriteEffects.None, layer);
            destination.Width = (int)Size.X - 2 * PatchSize;
            destination.X += PatchSize;
            SpriteBatch.Draw(Texture, destination, middleCenterSrc, Color.White, 0, new Vector2(0,0), SpriteEffects.None, layer);
            destination.X += destination.Width;
            destination.Width = PatchSize;
            SpriteBatch.Draw(Texture, destination, middleRightSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X = (int)AbsolutePosition.X;
            destination.Y += destination.Height;
            destination.Height = PatchSize;
            SpriteBatch.Draw(Texture, destination, bottomLeftSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X += PatchSize;
            destination.Width = (int)Size.X - 2 * PatchSize;
            SpriteBatch.Draw(Texture, destination, bottomCenterSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            destination.X += destination.Width;
            destination.Width = PatchSize;
            SpriteBatch.Draw(Texture, destination, bottomRightSrc, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, layer);
            
        }
        protected virtual void DrawSelf(GameTime gameTime)
        {

        }
        public virtual void Draw(GameTime gameTime)
        {
            if (!TrasnparentBackground)
            {
                DrawBackground();
            }
            DrawSelf(gameTime);
            foreach (var control in Controls)
            {
                control.Draw(gameTime);
            }
        }
    }
}
