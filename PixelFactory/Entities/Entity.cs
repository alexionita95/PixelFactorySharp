using Microsoft.Xna.Framework;

namespace PixelFactory.Entities
{
    public class Entity
    {

        public string Id { get; set; }
        public EntityManager EntityManager { get; set; }

        public Entity() 
        {
        }
        public virtual void Dispose()
        {

        }
        public Entity(Entity entity)
        {
            Id = entity.Id;
            EntityManager = entity.EntityManager;
        }
        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
