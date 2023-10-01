using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using PixelFactory.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class Player : Entity
    {
        public ulong InventorySize { get; set; } = 10;
        public Inventory Inventory { get; set; }
        public Crafter Crafter { get; set; }
        public Player()
        {
            Inventory = new Inventory(InventorySize);
            Crafter = new Crafter();
        }

        public bool HasNecessaryItems(Recipe recipe)
        {
            foreach (RecipeItem item in recipe.Inputs)
            {
                if (!Inventory.HasItems(item.Item, item.Quantity))
                {
                    return false;
                }
            }
            return true;
        }

        public void Craft(Recipe recipe, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (HasNecessaryItems(recipe))
                {
                    foreach (RecipeItem item in recipe.Inputs)
                    {
                        Inventory.RemoveItem(item.Item, item.Quantity);
                    }
                    Crafter.Enqueue(recipe);
                }
            }
        }

        public void AddItemToInventory(Item item)
        {
            Inventory.AddItem(item);
        }
        public void AddItemsToInventory(Item item, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Inventory.AddItem(item);
            }
        }

        public void DisplayDebugInventory()
        {
            Debug.WriteLine("##########Inventory##########");
            foreach (ItemSlot slot in Inventory.Slots)
            {
                if (!slot.IsEmpty)
                {
                    Debug.WriteLine($"#{slot.FilterItem.Id} Quantity: {slot.Count}");
                }
                else
                {
                    Debug.WriteLine($"<Empty Slot>");
                }
            }
            Debug.WriteLine("#############################");
        }

        public override void Update(GameTime gameTime)
        {
            Inventory.Update(gameTime);
            Crafter.Update(gameTime);
            if (Crafter.HasOutputItems)
            {
                while (Crafter.HasOutputItems && !Inventory.IsFull)
                {
                    ItemSlot output = Crafter.GetFirstOutput();
                    if (!Inventory.IsFull && Inventory.CanAccept(output.FilterItem, output.Count))
                    {
                        Inventory.AddItems(output.FilterItem, output.Count);
                        Crafter.RemoveFirstOutput();
                    }
                    else
                    {
                        break;
                    }
                }
                DisplayDebugInventory();
            }
            base.Update(gameTime);
        }
    }
}
