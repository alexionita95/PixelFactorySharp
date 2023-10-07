using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class CraftingJob : Entity
    {
        public Recipe Recipe { get; private set; }
        public Queue<InventorySlot> Outputs { get; private set; }
        public bool HasOutputItems { get => Outputs.Count > 0; }
        public bool Finished { get; private set; }
        public double Progress { get; private set; } = 0;
        public CraftingJob() 
        { 
            Finished = false;
            Outputs = new Queue<InventorySlot>();
            Progress = 0;
        }
        public CraftingJob(Recipe recipe)
        {
            Recipe = recipe;
            Finished = false;
            Outputs = new Queue<InventorySlot>();
            Progress = 0;
        }
        public InventorySlot GetOutput()
        {
            return Outputs.Dequeue();
        }
        public override void Update(GameTime gameTime)
        {
            if(Finished)
            {
                return;
            }
            double step = gameTime.ElapsedGameTime.TotalMilliseconds / Recipe.Duration;
            Progress += step;
            if (Progress >= 1.0)
            {
                foreach (RecipeItem item in Recipe.Outputs)
                {
                    Outputs.Enqueue(new InventorySlot(item.Item, item.Quantity));
                }
                Finished = true;
            }
            base.Update(gameTime);
        }
    }
}
