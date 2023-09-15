using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class EntityManager
    {
        private List<Entity> entities;
        public EntityManager() 
        {
            entities = new List<Entity>();
        }

        public void Add(Entity entity) 
        { 
            entities.Add(entity);
            entity.EntityManager = this;
        }

        public bool isInBounds(Vector2 position, Rectangle bounds)
        {
            return position.X >= bounds.Left && position.X < bounds.Right && position.Y >= bounds.Top && position.Y < bounds.Bottom;
        }
        public Entity GetFromPosition(Vector2 positon)
        {
            foreach (var entity in entities)
            {
                if(entity is DrawableEntity)
                {
                    var drawable = entity as DrawableEntity;
                    var bounds = new Rectangle(drawable.Position.ToPoint(), drawable.Size.ToPoint());
                    if (isInBounds(positon, bounds))
                    {
                        return entity;
                    }
                }
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                entity.Update(gameTime);
            }
        }
        public void Draw(GameTime gameTime) 
        {
            foreach(var entity in entities)
            {
                if (entity is DrawableEntity)
                {
                    var drawable = entity as DrawableEntity;
                    drawable.Draw(gameTime);
                }
            }
        }
    }
}
