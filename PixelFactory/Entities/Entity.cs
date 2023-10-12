using Microsoft.Xna.Framework;
using PixelFactory.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

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
        public virtual List<byte> GetData()
        {
            List<byte> data = new List<byte>();
            Serialization.Serializer.WriteString(Id, data);
            return data;
        }
        public virtual List<byte> Serialize() 
        {
            List<byte> result = new List<byte>();
            string name = this.GetType().FullName;
            Serialization.Serializer.WriteString(name, result);
            result.AddRange(this.GetData());
            return result;
        }
        public virtual void Deserialize(List<byte> data, ContentManager contentManager) 
        {
            Id = Serializer.ReadString(data);
        }
    }
}
