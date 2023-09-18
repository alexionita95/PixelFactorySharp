using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using PixelFactory.Items;
using PixelFactory.Entities;
using PixelFactory.Utils;

namespace PixelFactory.Logistics
{
    public class ItemLogisticsComponent : DrawableEntity
    {
        public int ItemCountLimit { get; set; } = 4;
        public List<ItemLogisticsComponentPort> Inputs { get; set; }
        public List<ItemLogisticsComponentPort> Outputs { get; set; }
        public float ProcessingTime { get; set; }
        public int CurrentOutputIndex { get; set; } = 0;
        public Vector2 ItemScale { get; set; }
        public ItemLogisticsComponent(SpriteBatch spriteBatch, Vector2 size)
       : base(spriteBatch, size)
        {
            Inputs = new List<ItemLogisticsComponentPort>();
            Outputs = new List<ItemLogisticsComponentPort>();
            Layer = DrawLayer.Buildings;
            ItemScale = new Vector2(.25f, .25f);
        }
        public bool IsFull()
        {
            int count = 0;
            foreach (var input in Inputs)
            {
                count += input.Items.Count;
            }
            foreach (var output in Outputs)
            {
                count += output.Items.Count;
            }
            return count >= ItemCountLimit;
        }
        public bool IsEmpty()
        {
            foreach (var input in Inputs)
                if (input.HasItems)
                    return false;
            foreach (var output in Outputs)
                if (output.HasItems)
                    return false;
            return true;
        }
        protected Vector2 GetItemPosition(LogisticsItem item)
        {
            var startPort = GetInput(item.SourceDirection, item.SourcePosition);
            var endPort = GetOutput(item.DestinationDirection, item.DestinationPosition);
            Vector2 startPos = DirectionUtils.GetInsideEdgePosition(startPort.Direction,startPort.Position, Rotation,rotatedSize, item.Scale);
            Vector2 endPos = DirectionUtils.GetInsideEdgePosition(endPort.Direction, endPort.Position, Rotation, rotatedSize, item.Scale);
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
                Vector2 midPoint = HelperFunctions.GetMidPoint(startPos, endPos, rotatedSize, ItemScale/2);
                interpolator.AddPoint(midPoint);
                interpolator.AddPoint(endPos);
                return interpolator.Interpolate(item.Progress);
            }
        }
        public bool ValidatePositionOnPorts(Direction direction, List<ItemLogisticsComponentPort> ports, uint position = 0)
        {
            foreach (var port in ports)
            {
                var rotatedDirection = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(port.Direction, port.Position, Rotation, rotatedSize);
                if (rotatedDirection == direction && rotatedPosition == position)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsPositionValid(Direction direction, ItemLogisticsComponentPort.PortType type, uint position = 0)
        {
            switch (direction)
            {
                case Direction.N:
                case Direction.S:
                    if (rotatedSize.X - 1 < position) return false;
                    break;
                case Direction.E:
                case Direction.W:
                    if (rotatedSize.Y - 1 < position) return false;
                    break;
            }
            switch (type)
            {
                case ItemLogisticsComponentPort.PortType.Input:
                    return ValidatePositionOnPorts(direction, Inputs, position);
                case ItemLogisticsComponentPort.PortType.Output:
                    return ValidatePositionOnPorts(direction, Outputs, position);
                default:
                    break;
            }
            return true;
        }
        public void AddInput(Direction direction, uint position = 0)
        {
            if (IsPositionValid(direction, ItemLogisticsComponentPort.PortType.Input, position))
            {
                ItemLogisticsComponentPort port = new ItemLogisticsComponentPort(ItemLogisticsComponentPort.PortType.Input, direction) { Position = position };
                Inputs.Add(port);
            }
        }
        public void AddOutput(Direction direction, uint position = 0)
        {
            if (IsPositionValid(direction, ItemLogisticsComponentPort.PortType.Input, position))
            {
                ItemLogisticsComponentPort port = new ItemLogisticsComponentPort(ItemLogisticsComponentPort.PortType.Output, direction) { Position = position };
                Outputs.Add(port);
            }

        }
        public ItemLogisticsComponentPort GetInput(Direction direction, uint position = 0)
        {
            foreach (var input in Inputs)
            {
                if (input.Direction == direction && input.Position == position) return input;
            }
            return null;
        }
        public ItemLogisticsComponentPort GetOutput(Direction direction, uint position = 0)
        {
            foreach (var output in Outputs)
            {
                if (output.Direction == direction && output.Position == position) return output;
            }
            return null;
        }
        public void AddItemToInput(Item item, Direction direction, Vector2 position)
        {

            var portDir = DirectionUtils.GetRotatedDirection(direction, Rotation, true);
            uint portPosition = DirectionUtils.CalculatePortPosition(portDir, position, Position, rotatedSize);
            var port = GetInput(portDir, portPosition);
            if (port != null)
            {
                if (IsFull())
                {
                    return;
                }
                LogisticsItem logisticsComponentItem = new LogisticsItem(item, spriteBatch);
                logisticsComponentItem.SourcePosition = port.Position;
                logisticsComponentItem.SourceDirection = port.Direction;
                var currentOutputPort = Outputs[CurrentOutputIndex];
                logisticsComponentItem.DestinationDirection = currentOutputPort.Direction;
                logisticsComponentItem.DestinationPosition = currentOutputPort.Position;
                logisticsComponentItem.Position = Position;
                logisticsComponentItem.Scale = ItemScale;
                port.Items.Add(logisticsComponentItem);
            }
        }
        public void AddItemToOutput(Item item, Direction direction, Vector2 position)
        {
            uint portPosition = DirectionUtils.CalculatePortPosition(direction, position, Position, rotatedSize);
            var port = GetOutput(direction, portPosition);

            if (port != null)
            {
                if (port.IsFull())
                {
                    return;
                }
                LogisticsItem logisticsComponentItem = new LogisticsItem(item, spriteBatch);
                port.Items.Add(logisticsComponentItem);
            }
        }
        public bool CanAcceptItemsFrom(Direction direction, Vector2 position)
        {
            uint portPosition = DirectionUtils.CalculatePortPosition(direction, position, Position, rotatedSize);
            foreach (var input in Inputs)
            {
                var rotatedDirection = DirectionUtils.GetRotatedDirection(input.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(input.Direction, input.Position, Rotation, rotatedSize);
                if (rotatedDirection == direction && rotatedPosition == portPosition)
                {
                    if (!IsFull() && CanMove(DirectionUtils.GetInsideEdgePosition(input.Direction, input.Position, Rotation, rotatedSize, ItemScale)))
                        return true;
                }
            }
            return false;
        }
        public bool CanExportItemsTo(Direction direction, Vector2 position)
        {
            uint portPosition = DirectionUtils.CalculatePortPosition(direction, position, Position, rotatedSize);
            foreach (var output in Outputs)
            {
                var rotatedDirection = DirectionUtils.GetRotatedDirection(output.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(output.Direction, output.Position, Rotation, rotatedSize);
                if (rotatedDirection == direction && rotatedPosition == portPosition)
                {

                    if (output.HasItems)
                        return true;
                }
            }
            return false;
        }
        public float CheckDistance(float lastDistance, Vector2 start, Vector2 end)
        {
            float result = lastDistance;
            float dist = Vector2.Distance(start, end);
            if (dist < result)
            {
                result = dist;
            }
            return result;
        }
        public float GetClosestDistance(List<ItemLogisticsComponentPort> ports, Vector2 position, LogisticsItem filter = null)
        {
            float lastDist = float.MaxValue;

            foreach (var port in ports)
            {
                if (port.HasItems)
                {
                    foreach (var it in port.Items)
                    {
                        if(filter == null)
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
                }
            }
            return lastDist;
        }
        public float GetClosestItem(Vector2 position, LogisticsItem filter = null)
        {
            float lastDist;
            lastDist = GetClosestDistance(Inputs, position, filter);
            if (lastDist == float.MaxValue)
            {
                lastDist = GetClosestDistance(Outputs, position, filter);
            }
            return lastDist;
        }
        public bool CanMove(Vector2 position, LogisticsItem filter = null)
        {
            if (IsEmpty())
            {
                return true;
            }
            var pos = GetClosestItem(position, filter);
            if (pos == float.MaxValue)
                return true;
            if (pos < ItemScale.X)
                return false;
            return true;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            double step = gameTime.ElapsedGameTime.TotalMilliseconds / ProcessingTime;
            foreach (var output in Outputs)
            {
                if (output.HasItems)
                {
                    List<LogisticsItem> toRemove = new List<LogisticsItem>();
                    foreach (var item in output.Items)
                    {
                        Vector2 pos = DirectionUtils.GetNextPosition(output.Direction, output.Position, Rotation, Position, rotatedSize);
                        Entity entity = EntityManager.GetFromPosition(pos);
                        if (entity is ItemLogisticsComponent)
                        {
                            var component = entity as ItemLogisticsComponent;
                            var dir = DirectionUtils.GetRotatedDirection(output.Direction, Rotation);
                            var opposite = DirectionUtils.GetOppositeDirection(dir);
                            if (component.CanAcceptItemsFrom(opposite, pos))
                            {
                                component.AddItemToInput(item.Item, opposite, pos);
                                toRemove.Add(item);
                            }
                        }
                    }
                    foreach (var remove in toRemove)
                    {
                        output.Items.Remove(remove);
                    }
                }
            }
            foreach (var input in Inputs)
            {
                if (input.HasItems)
                {
                    List<LogisticsItem> toRemove = new List<LogisticsItem>();
                    foreach (var item in input.Items)
                    {
                        if (CanMove(GetItemPosition(item), item))
                        {
                            item.Update(step);
                        }
                        if (item.Ready)
                        {
                            var output = GetOutput(item.DestinationDirection, item.DestinationPosition);
                            if (output != null && !output.IsFull())
                            {
                                output.Items.Add(item);
                                toRemove.Add(item);
                            }
                        }
                    }
                    if (toRemove.Count > 0)
                    {
                        foreach (var remove in toRemove)
                        {
                            input.Items.Remove(remove);
                        }
                    }
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
