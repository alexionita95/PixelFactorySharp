using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Animations
{
    public class AnimationManager
    {
        private List<Animation> animations;
        public AnimationManager()
        {
            animations = new List<Animation>();
        }
        public void AddAnimation(Animation animation)
        {
            animations.Add(animation);
        }
        public void Update(GameTime gameTime)
        {
            foreach (var animation in animations)
            {
                animation.Update(gameTime);
            }
        }
    }
}
