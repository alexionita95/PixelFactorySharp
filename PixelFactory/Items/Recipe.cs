using PixelFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Items
{
    public class RecipeItem
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public double Chance { get; set; } = 1;
        public RecipeItem()
        {

        }
        public RecipeItem(Item item,int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
    public class Recipe : Entity
    {
        public List<RecipeItem> Inputs { get; set;}
        public List<RecipeItem> Outputs { get; set;}
        public double Duration { get; set; }
        public Recipe() 
        {
            Inputs = new List<RecipeItem>();
            Outputs = new List<RecipeItem>();
        }
        public void AddInput(RecipeItem input) 
        {
            Inputs.Add(input);
        }
        public void AddOutput(RecipeItem output)
        {
            Outputs.Add(output);
        }
    }
}
