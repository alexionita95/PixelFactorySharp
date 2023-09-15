using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
