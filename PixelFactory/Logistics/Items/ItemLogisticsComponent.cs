using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using PixelFactory.Inventory;
using PixelFactory.Entities;
using PixelFactory.Utils;

namespace PixelFactory.Logistics.Items
{
    public class ItemLogisticsComponent : LogisticsComponent
    {
        public List<LogisticsEntity> Items { get; set; }
        public Vector2 ItemScale { get; set; }
        public int CurrentOutputIndex { get; set; } = 0;
        public ItemLogisticsComponent(Vector2 size)
 : base(size)
        {
            ItemScale = new Vector2(.25f, .25f);
            Items = new List<LogisticsEntity>();
        }
        public ItemLogisticsComponent()
        {
            ItemScale = new Vector2(.25f, .25f);
            Items = new List<LogisticsEntity>();
        }

        public override bool IsFull()
        {
            return Items.Count == Capacity;
        }
        public override bool IsEmpty()
        {
            return Items.Count == 0;
        }
        public override float Count()
        {
            return Items.Count;
        }

        protected Vector2 GetItemPosition(LogisticsEntity item)
        {
            var startPort = GetInput(item.SourceDirection, item.SourcePosition);
            var endPort = GetOutput(item.DestinationDirection, item.DestinationPosition);
            Vector2 startPos = DirectionUtils.GetInsideEdgePosition(startPort.Direction, startPort.Position, Rotation, RotatedSize, item.Scale);
            Vector2 endPos = DirectionUtils.GetInsideEdgePosition(endPort.Direction, endPort.Position, Rotation, RotatedSize, item.Scale);
            bool straight = HelperFunctions.IsStraightLine(startPos, endPos);
            PathInterpolator interpolator = new PathInterpolator();
            interpolator.AddPoint(startPos);
            if (straight)
            {
                interpolator.AddPoint(endPos);
                return interpolator.Interpolate(item.Progress);
            }
            else
            {
                Vector2 midPoint = HelperFunctions.GetMidPoint(startPos, endPos, RotatedSize, ItemScale / 2);
                interpolator.AddPoint(midPoint);
                interpolator.AddPoint(endPos);
                return interpolator.Interpolate(item.Progress);
            }
        }

        public override void Add(InventoryEntity entity, Direction direction, Vector2 position)
        {

            var portDir = DirectionUtils.GetRotatedDirection(direction, Rotation, true);
            uint portPosition = DirectionUtils.CalculatePortPosition(portDir, position, Position, RotatedSize);
            var port = GetInput(portDir, portPosition);
            if (port != null)
            {
                if (IsFull())
                {
                    return;
                }
                LogisticsEntity logisticsComponentItem = new LogisticsEntity(entity);
                logisticsComponentItem.SourcePosition = port.Position;
                logisticsComponentItem.SourceDirection = port.Direction;
                var currentOutputPort = Ports[CurrentOutputIndex];
                logisticsComponentItem.DestinationDirection = currentOutputPort.Direction;
                logisticsComponentItem.DestinationPosition = currentOutputPort.Position;
                logisticsComponentItem.Position = Position;
                logisticsComponentItem.Scale = ItemScale;
                Items.Add(logisticsComponentItem);
            }
        }

        public override bool CanAcceptFrom(InventoryEntityType entityType, Direction direction, Vector2 position)
        {
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
                    if (!IsFull() && CanMove(DirectionUtils.GetInsideEdgePosition(port.Direction, port.Position, Rotation, RotatedSize, ItemScale)))
                        return true;
                }
            }
            return false;
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

        public float CheckDistance(float lastDistance, Vector2 start, Vector2 end)
        {
            float result = lastDistance;
            float dist = Vector2.DistanceSquared(start, end);
            if (dist < result)
            {
                result = dist;
            }
            return result;
        }
        public float GetClosestDistance(Vector2 position, LogisticsEntity filter = null)
        {
            float lastDist = float.MaxValue;

            foreach (var it in Items)
            {
                if (filter == null)
                {
                    lastDist = CheckDistance(lastDist, position, GetItemPosition(it));
                }
                else
                {
                    if (it != filter && it.Progress > filter.Progress)
                    {
                        lastDist = CheckDistance(lastDist, position, GetItemPosition(it));
                    }
                }
            }
            return lastDist;
        }
        public float GetClosestItem(Vector2 position, LogisticsEntity filter = null)
        {
            float lastDist;
            lastDist = GetClosestDistance(position, filter);
            return lastDist;
        }
        public bool CanMove(Vector2 position, LogisticsEntity filter = null)
        {
            if (IsEmpty())
            {
                return true;
            }
            var pos = GetClosestItem(position, filter);
            if (pos == float.MaxValue)
                return true;
            if (pos < ItemScale.X * ItemScale.X * 1.5)
                return false;
            return true;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            double step = gameTime.ElapsedGameTime.TotalMilliseconds / ProcessingTime;
            foreach (var item in Items)
            {
                if (CanMove(GetItemPosition(item), item))
                {
                    item.Update(step);
                }
            }
            List<LogisticsEntity> toRemove = new List<LogisticsEntity>();
            foreach (var item in Items)
            {
                if (!item.ReachedDestination)
                {
                    continue;
                }
                Vector2 pos = DirectionUtils.GetNextPosition(item.DestinationDirection, item.DestinationPosition, Rotation, Position, RotatedSize);
                Entity entity = EntityManager.GetFromPosition(pos);
                if (entity is ItemLogisticsComponent)
                {
                    var component = entity as ItemLogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(item.DestinationDirection, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (component.CanAcceptFrom(item.Entity.Type, opposite, pos))
                    {
                        component.Add(item.Entity, opposite, pos);
                        toRemove.Add(item);
                    }
                }
            }
            foreach (var remove in toRemove)
            {
                Items.Remove(remove);
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }


    }
}
