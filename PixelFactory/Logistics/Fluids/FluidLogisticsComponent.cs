using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using PixelFactory.Logistics.Items;
using PixelFactory.Serialization;
using PixelFactory.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Logistics.Fluids
{
    public class FluidLogisticsComponent : LogisticsComponent
    {
        public InventoryEntity CurrentEntity { get; private set; }
        public bool HasNetwork { get => Network != null; }
        public FluidLogisticsNetwork Network { get; set; }
        private float currentCount;
        public FluidLogisticsComponent()
        {
            currentCount = 0;
            CurrentEntity = null;
            Network = null;
        }
        public FluidLogisticsComponent(Vector2 size) : base(size)
        {
            currentCount = 0;
            CurrentEntity = null;
            Network = null;
        }
        public void Flush()
        {
            currentCount = 0;
        }
        public override float Count()
        {
            return currentCount;
        }
        public override bool IsFull()
        {
            if (!HasNetwork)
            {
                return currentCount == Capacity;
            }
            return Network.IsFull;
        }
        public override bool IsEmpty()
        {
            if (!HasNetwork)
            {
                return currentCount == 0;
            }

            return Network.IsEmpty;
        }
        public override void Dispose()
        {
            Network.Remove(this);
            base.Dispose();
        }
        private void Reset()
        {
            if (currentCount > 0)
            {
                return;
            }
            if (CurrentEntity != null)
            {
                CurrentEntity = null;
                currentCount = 0;
            }
        }
        public void ResetNetwork()
        {
            Network = null;
        }

        public Direction GetFlowDirection(EntityRotation entityRotation)
        {
            switch (entityRotation)
            {
                case EntityRotation.None:
                case EntityRotation.Rot180:
                    return Direction.S;
                case EntityRotation.Rot90:
                case EntityRotation.Rot270:
                    return Direction.W;
            }
            return Direction.S;
        }
        public bool IsInFlowDirection(Direction flowDirection, EntityRotation entityRotation)
        {
            var flowDirectionFromRotation = GetFlowDirection(entityRotation);
            switch (flowDirection)
            {
                case Direction.N:
                case Direction.S:
                    if (flowDirectionFromRotation == Direction.S)
                    {
                        return true;
                    }
                    break;
                case Direction.W:
                case Direction.E:
                    if (flowDirectionFromRotation == Direction.W)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public int CountValidPorts()
        {
            InventoryEntityType entityType = InventoryEntityType.Fluid;
            int result = 0;
            foreach (var port in Ports)
            {
                Vector2 pos = DirectionUtils.GetNextPosition(port.Direction, port.Position, Rotation, Position, RotatedSize);
                Entity entity = EntityManager.GetFromPosition(pos);
                if (entity is LogisticsComponent)
                {
                    var logisticsEntity = entity as LogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (logisticsEntity.CanAcceptFrom(entityType, opposite, pos))
                    {
                        if (IsInFlowDirection(dir, logisticsEntity.Rotation))
                        {
                            result++;
                        }
                        else
                        {
                            if (HelperFunctions.IsPerpendicular(Rotation, logisticsEntity.Rotation))
                            {
                                result++;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public Dictionary<Direction, LogisticsComponent> GetConnectedEntites()
        {
            InventoryEntityType entityType = InventoryEntityType.Fluid;
            Dictionary<Direction, LogisticsComponent> result = new Dictionary<Direction, LogisticsComponent>();
            foreach (var port in Ports)
            {
                Vector2 pos = DirectionUtils.GetNextPosition(port.Direction, port.Position, Rotation, Position, RotatedSize);
                Entity entity = EntityManager.GetFromPosition(pos);
                if (entity is LogisticsComponent)
                {
                    var logisticsEntity = entity as LogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (logisticsEntity.CanAcceptFrom(entityType, opposite, pos))
                    {
                        if (IsInFlowDirection(dir, logisticsEntity.Rotation))
                        {
                            result.Add(dir, logisticsEntity);
                        }
                        else
                        {
                            if (HelperFunctions.IsPerpendicular(Rotation, logisticsEntity.Rotation))
                            {
                                result.Add(dir, logisticsEntity);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<FluidLogisticsNetwork> GetConnectedNetworks()
        {
            InventoryEntityType entityType = InventoryEntityType.Fluid;
            List<FluidLogisticsNetwork> result = new List<FluidLogisticsNetwork>();
            foreach (var port in Ports)
            {
                Vector2 pos = DirectionUtils.GetNextPosition(port.Direction, port.Position, Rotation, Position, RotatedSize);
                var entity = EntityManager.GetFromPosition(pos);
                if (entity is FluidLogisticsComponent)
                {
                    var logisticsEntity = entity as FluidLogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (logisticsEntity.CanAcceptFrom(entityType, opposite, pos))
                    {
                        if (IsInFlowDirection(dir, logisticsEntity.Rotation))
                        {
                            if (logisticsEntity.HasNetwork && !result.Contains(logisticsEntity.Network))
                            {
                                result.Add(logisticsEntity.Network);
                            }
                        }
                        else
                        {
                            if (HelperFunctions.IsPerpendicular(Rotation, logisticsEntity.Rotation))
                            {
                                if (logisticsEntity.HasNetwork && !result.Contains(logisticsEntity.Network))
                                {
                                    result.Add(logisticsEntity.Network);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public override bool CanExportTo(InventoryEntityType entityType, Direction direction, Vector2 position)
        {
            uint portPosition = DirectionUtils.CalculatePortPosition(direction, position, Position, RotatedSize);
            foreach (var port in Ports)
            {
                if (port.IsInput)
                {
                    continue;
                }
                var rotatedDirection = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(port.Direction, port.Position, Rotation, RotatedSize);
                if (rotatedDirection == direction && rotatedPosition == portPosition && port.EntityType == entityType)
                {
                    return true;
                }
            }
            return false;
        }
        public override bool CanExportTo(InventoryEntityType entityType, float count, Direction direction, Vector2 position)
        {
            if (count > Count())
            {
                return false;
            }
            return CanExportTo(entityType, direction, position);
        }
        public override bool CanAcceptFrom(InventoryEntityType entityType, Direction direction, Vector2 position)
        {
            if (CurrentEntity != null)
            {
                if (CurrentEntity.Type != entityType)
                {
                    return false;
                }
            }
            uint portPosition = DirectionUtils.CalculatePortPosition(direction, position, Position, RotatedSize);
            foreach (var port in Ports)
            {
                if (port.IsOutput)
                {
                    continue;
                }
                var rotatedDirection = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(port.Direction, port.Position, Rotation, RotatedSize);
                if (rotatedDirection == direction && rotatedPosition == portPosition && port.EntityType == entityType)
                {
                        return true;
                }
            }
            return false;
        }
        public override bool CanAcceptFrom(InventoryEntityType entityType, float count, Direction direction, Vector2 position)
        {
            if (Network != null)
            {
                return Network.CanAcceptFluid(entityType, count);
            }
            if (count > AvailableSpace)
            {
                return false;
            }
            return CanAcceptFrom(entityType, direction, position);
        }

        public override void Add(InventoryEntity entity, float count, Direction direction, Vector2 position)
        {
            if (!CanAcceptFrom(entity.Type, count, direction, position))
            {
                return;
            }
            if (CurrentEntity == null)
            {
                CurrentEntity = new InventoryEntity(entity);
            }
            if(CurrentEntity.Type != entity.Type)
            {
                return;
            }
            var portDir = DirectionUtils.GetRotatedDirection(direction, Rotation, true);
            uint portPosition = DirectionUtils.CalculatePortPosition(portDir, position, Position, RotatedSize);
            var port = GetInput(portDir, portPosition);
            if (port != null)
            {
                if (IsFull())
                {
                    return;
                }
                if (Network != null)
                {
                    Network.AddFluid(entity, count);
                }
                else
                {
                    currentCount += count;
                }
            }
        }
        private void FindNetwork()
        {
            var networks = GetConnectedNetworks();
            if (networks.Count == 0)
            {
                Network = new FluidLogisticsNetwork();
                Network.Add(this);
                if (currentCount > 0)
                {
                    Network.AddFluid(CurrentEntity, currentCount);
                    Flush();
                }
               
                return;
            }
            if (networks.Count == 1)
            {
                networks.First().Add(this);
                return;
            }
            if (networks.Count > 1)
            {
                for (int i = 1; i < networks.Count; ++i)
                {
                    networks[0].Merge(networks[i]);
                }
                networks.First().Add(this);
            }

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!HasNetwork)
            {
                FindNetwork();
                return;
            }
            Network.Update(gameTime);
            if(Network.IsEmpty)
            {
                return;
            }
            CurrentEntity = Network.CurrentEntity;
            float fluidInSegment = Network.FluidInSegment;
            if(fluidInSegment > Capacity)
            {
                fluidInSegment = Capacity;
            }
            currentCount = Network.GetFluid(fluidInSegment);

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public override List<byte> GetData()
        {
            List<byte> data = base.GetData();
            Serializer.WriteString(CurrentEntity.Id, data);
            Serializer.WriteFloat(currentCount, data);
            return data;
        }

    }
}
