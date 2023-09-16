﻿using Microsoft.Xna.Framework;

namespace PixelFactory
{
    public class Entity
    {

        public string Id { get; set; }
        public EntityManager EntityManager { get; set; }
        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
