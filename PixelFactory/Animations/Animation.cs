using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PixelFactory.Animations
{
    public class Animation
    {
        public Texture2D Texture { get; set; }
        public Vector2 FrameSize { get; set; }
        public int CurrentFrameIndex {  get; private set; } = 0;
        public double Duration { get; private set; }
        public Rectangle CurrentFrame { get => CalculateCurrentFrame(); }
        private int FrameCount = 0;
        private int RowCount = 0;
        public int CurrentRow { get; set; }
        private double progress;
        private bool playing = true;
        public bool Loop { get; private set; }

        public Animation(Texture2D texture, Vector2 frameSize, double duration, bool loop = false) 
        {
            Texture = texture;
            FrameSize = frameSize;
            Duration = duration;
            FrameCount = (int)(Texture.Width / frameSize.X);
            RowCount = (int)(Texture.Height / frameSize.Y);
            CurrentFrameIndex = 0;
            CurrentRow = 0;
            progress = 0;
            Loop = loop;
            
        }
        private Rectangle CalculateCurrentFrame()
        {
            return new Rectangle(new Vector2(FrameSize.X * CurrentFrameIndex, FrameSize.Y * CurrentRow).ToPoint(), FrameSize.ToPoint());
        }
        public void Update(GameTime gameTime)
        {
            if(!playing)
            {
                if(CurrentFrameIndex != 0)
                {
                    CurrentFrameIndex = 0;
                }
                return;
            }
            progress += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (progress > Duration)
            {
                if (Loop)
                {
                    progress = 0;
                }
                else
                {
                    playing = false;
                }
                
            }
            int frameIndex = (int)Utils.HelperFunctions.Remap(progress, 0, Duration, 0, FrameCount);
            if(frameIndex != CurrentFrameIndex)
            {
                CurrentFrameIndex = frameIndex;
            }

        }
    }
}
