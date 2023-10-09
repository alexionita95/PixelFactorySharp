using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Inventory
{
    public enum InventoryEntityType
    {
        Solid, Fluid
    }
    public class InventoryEntity : DrawableEntity
    {

        public string Name { get; set; }
        public InventoryEntityType Type { get; set; } = InventoryEntityType.Solid;
        public int MaximumQuantity { get; set; } = 10;
        public InventoryEntity() : base(Vector2.One)
        {
        }
        public InventoryEntity(InventoryEntity item): base(item)
        {
            Name = item.Name;
            MaximumQuantity = item.MaximumQuantity;
            Type = item.Type;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
        public bool HasSameId(InventoryEntity item)
        {
            if (item == null)
            {
                return false;
            }
            return item.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is InventoryEntity)) return false;
            InventoryEntity item = (InventoryEntity)obj;
            return this.Id == item.Id;
            
        }

        public override int GetHashCode()
        {
           return Id.GetHashCode() + MaximumQuantity.GetHashCode() + base.GetHashCode();
        }
    }
}
