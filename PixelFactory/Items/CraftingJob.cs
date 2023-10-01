﻿using Microsoft.Xna.Framework;
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
        public Queue<ItemSlot> Outputs { get; private set; }
        public bool HasOutputItems { get => Outputs.Count > 0; }
        public bool Finished { get; private set; }
        private double progress = 0;
        public CraftingJob() 
        { 
            Finished = false;
            Outputs = new Queue<ItemSlot>();
            progress = 0;
        }
        public CraftingJob(Recipe recipe)
        {
            Recipe = recipe;
            Finished = false;
            Outputs = new Queue<ItemSlot>();
            progress = 0;
        }
        public ItemSlot GetOutput()
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
            progress += step;
            if (progress >= 1.0)
            {
                foreach (RecipeItem item in Recipe.Outputs)
                {
                    Outputs.Enqueue(new ItemSlot(item.Item, item.Quantity));
                }
                Finished = true;
            }
            base.Update(gameTime);
        }
    }
}