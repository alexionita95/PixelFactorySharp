using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using PixelFactory.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static PixelFactory.Entities.DrawableEntity;

namespace PixelFactory.Logistics.Fluids
{
    public class FluidLogisticsNetwork
    {
        public List<FluidLogisticsComponent> Components { get; private set; }
        public InventoryEntity CurrentEntity { get; private set; } = null;
        public float Capacity { get; private set; } = 0;
        public float Count { get; private set; } = 0;
        public float AvailableSpace { get => Capacity - Count; }
        public float FluidInSegment { get; private set; } = 0;
        public bool IsFull { get => Count != 0 && Count == Capacity; }
        public bool IsEmpty { get => Count == 0; }
        private float availableFluid = 0;
        public FluidLogisticsNetwork()
        {
            Components = new List<FluidLogisticsComponent>();
        }

        public Entity GetFromPosition(Vector2 positon)
        {
            foreach (var entity in Components)
            {
                if (entity is DrawableEntity)
                {
                    var drawable = entity as DrawableEntity;
                    var bounds = new Rectangle(drawable.Position.ToPoint(), drawable.Size.ToPoint());
                    if (HelperFunctions.IsInBounds(positon, bounds))
                    {
                        return entity;
                    }
                }
            }
            return null;
        }
        private void CalculateFluidInSegment()
        {
            if (Components.Count > 0 && Count > 0)
            {
                FluidInSegment = Count / Components.Count;
                return;
            }
            FluidInSegment = 0;
        }
        public void Remove(FluidLogisticsComponent component)
        {
            Components.Remove(component);
            Capacity -= component.Capacity;
            if (Components.Count > 1)
            {
                for(int i=1; i<Components.Count; ++i) 
                {
                    Components[i].ResetNetwork();
                }
                FluidLogisticsComponent first = Components.First();
                Clear();
                Add(first);
            }
            if (Components.Count == 0)
            {
                Clear();
            }
        }
        public bool Add(FluidLogisticsComponent component)
        {
            if (Components.Contains(component))
            { 
                return false; 
            }
            Components.Add(component);
            Capacity += component.Capacity;
            if (!component.IsEmpty())
            {
                if (CanAcceptFluid(component.CurrentEntity.Type, component.Count()))
                {
                    AddFluid(component.CurrentEntity, component.Count());
                    component.Flush();
                }
            }
            CalculateFluidInSegment();
            component.Network = this;
            return true;
        }
        public bool CanAcceptFluid(InventoryEntityType entityType, float count)
        {
            if (count > AvailableSpace)
            {
                return false;
            }
            if (CurrentEntity != null && CurrentEntity.Type != entityType)
            {
                return false;
            }
            return true;
        }
        public void AddFluid(InventoryEntity entity, float count)
        {
            if (CurrentEntity == null)
            {
                CurrentEntity = new InventoryEntity(entity);
            }
            if (entity.Type != CurrentEntity.Type)
            {
                return;
            }
            Count += count;
            CalculateFluidInSegment();

        }
        public void Clear()
        {
            Components.Clear();
            Capacity = 0;
            Count = 0;
            CurrentEntity = null;
            FluidInSegment = 0;
            availableFluid = 0;
        }
        public void Merge(FluidLogisticsNetwork network)
        {
            foreach (var component in network.Components)
            {
                Add(component);
            }
        }
        public float GetFluid(float count)
        {
            if (availableFluid == 0)
                return 0;
            availableFluid -= count;
            return count;
        }
        public void Update(GameTime gameTime)
        {
            if(Count > 0 && availableFluid <= 0)
            {
                availableFluid = Count;
            }

        }
    }
}
