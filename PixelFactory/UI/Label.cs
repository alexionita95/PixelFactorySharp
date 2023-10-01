using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace PixelFactory.UI
{
    public class Label : UIControl
    {
        public string Text { get=>text; set { text = value; CalculateTextPosition(); } }
        public Color FontColor { get; set; } = Color.White;
        public SpriteFont Font { get; set; }

        public Vector2 TextPosition { get; private set; }
        private string text;
        public Label() 
        {
            Initialize();
        }
        private void Initialize()
        {
            TrasnparentBackground = true;
        }
        private void CalculateTextPosition()
        {
            Vector2 textSize = Font.MeasureString(Text);
            Size = textSize;
            TextPosition = new Vector2(AbsolutePosition.X + Size.X / 2 - textSize.X / 2, AbsolutePosition.Y + Size.Y / 2 - textSize.Y / 2);
        }
        public Label(string text, SpriteFont font)
        {
            Font = font;
            Text = text;
            Initialize();
        }
        public Label(Vector2 position, string text, SpriteFont font)
        {
            Position = position;
            Font = font;
            Text = text;
            Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CalculateTextPosition();
        }
        protected override void DrawSelf(GameTime gameTime)
        {
            int layer = 0;
            SpriteBatch.DrawString(Font, Text, TextPosition, Color.White,0, new Vector2(0, 0),1, SpriteEffects.None, layer);
        }
    }
}
