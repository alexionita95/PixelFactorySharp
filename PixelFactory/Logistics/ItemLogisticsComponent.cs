using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using PixelFactory.Items;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.Tracing;

namespace PixelFactory.Logistics
{
    public class ItemLogisticsComponent : DrawableEntity
    {
        public int ItemCountLimit { get; set; } = 10;
        public List<ItemLogisticsComponentPort> Inputs { get; set; }
        public List<ItemLogisticsComponentPort> Outputs { get; set; }
        public float ProcessingTime { get; set; }
        private int currentOutput = 0;
        public ItemLogisticsComponent(SpriteBatch spriteBatch, Vector2 size)
       : base(spriteBatch, size)
        {
            Inputs = new List<ItemLogisticsComponentPort>();
            Outputs = new List<ItemLogisticsComponentPort>();

        }
        private ItemLogisticsComponentPort.PortDirection IncrementDirection(ItemLogisticsComponentPort.PortDirection direction, int increment)
        {
            int length = Enum.GetValues(typeof(ItemLogisticsComponentPort.PortDirection)).Length;
            if (increment < 0) { increment += length; }

            return (ItemLogisticsComponentPort.PortDirection)(((int)direction + increment) % length);
        }
        public ItemLogisticsComponentPort.PortDirection GetRotatedDirection(ItemLogisticsComponentPort.PortDirection direction, bool back = false)
        {

            int increment = 0;
            switch (Rotation)
            {
                case EntityRotation.Rot90:
                    increment = 1;
                    break;
                case EntityRotation.Rot180:
                    increment = 2;
                    break;
                case EntityRotation.Rot270:
                    increment = 3;
                    break;
            }
            if (back)
            {
                increment *= -1;
            }
            return IncrementDirection(direction, increment);

        }
        private uint GetRotatedPosition(ItemLogisticsComponentPort.PortDirection direction, uint position)
        {
            var rotatedDir = GetRotatedDirection(direction);
            if (AreOpposite(direction, rotatedDir) || IsDirectionBefore(rotatedDir, direction))
            {
                switch (direction)
                {
                    case ItemLogisticsComponentPort.PortDirection.N:
                    case ItemLogisticsComponentPort.PortDirection.S:
                        return (uint)rotatedSize.X - position - 1;
                    case ItemLogisticsComponentPort.PortDirection.E:
                    case ItemLogisticsComponentPort.PortDirection.W:
                        return (uint)rotatedSize.Y - position - 1;
                }
            }
            return position;
        }
        bool AreOpposite(ItemLogisticsComponentPort.PortDirection direction, ItemLogisticsComponentPort.PortDirection other)
        {
            if ((direction == ItemLogisticsComponentPort.PortDirection.N && other == ItemLogisticsComponentPort.PortDirection.S) ||
                (direction == ItemLogisticsComponentPort.PortDirection.S && other == ItemLogisticsComponentPort.PortDirection.N) ||
                (direction == ItemLogisticsComponentPort.PortDirection.E && other == ItemLogisticsComponentPort.PortDirection.W) ||
                (direction == ItemLogisticsComponentPort.PortDirection.W && other == ItemLogisticsComponentPort.PortDirection.E))
            {
                return true;
            }
            return false;
        }
        bool IsDirectionBefore(ItemLogisticsComponentPort.PortDirection direction, ItemLogisticsComponentPort.PortDirection other)
        {
            if (IncrementDirection(direction, 1) == other)
                return true;
            return false;
        }

        protected Vector2 GetEdgePosition(ItemLogisticsComponentPort port)
        {

            var rotatedDirection = GetRotatedDirection(port.Direction);
            var rotatedPosition = GetRotatedPosition(port.Direction, port.Position) + 1;

            switch (rotatedDirection)
            {
                case ItemLogisticsComponentPort.PortDirection.N:
                    return new Vector2(rotatedPosition, 0);
                case ItemLogisticsComponentPort.PortDirection.S:
                    return new Vector2(rotatedPosition, rotatedSize.Y + 1);
                case ItemLogisticsComponentPort.PortDirection.E:
                    return new Vector2(0, rotatedPosition);
                case ItemLogisticsComponentPort.PortDirection.W:
                    return new Vector2(rotatedSize.X + 1, rotatedPosition);
            }

            return Vector2.Zero;
        }
        private float Interpolate(float start, float end, double progress)
        {
            return start + (end - start) * (float)progress;
        }
        private bool isStraightLine(Vector2 start, Vector2 end)
        {
            if (start.X == end.X || start.Y == end.Y) return true;
            return false;
        }
        protected bool IsOnEdge(float value)
        {
            if (value == 0 || value == 2) return true;
            return false;
        }
        protected Vector2 GetMidPoint(Vector2 start, Vector2 end)
        {
            float midX = 0;
            float midY = 0;
            if (IsOnEdge(start.X))
                midX = end.X;
            else
                midX = start.X;
            if (IsOnEdge(start.Y))
                midY = end.Y;
            else
                midY = start.Y;

            return new Vector2(midX, midY);
        }
        float Remap(float value, float min1, float max1, float min2, float max2)
        {
            return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
        }
        protected Vector2 GetItemPosition(LogisticsItem item)
        {
            var startPort = GetInput(item.SourceDirection, item.SourcePosition);
            var endPort = GetOutput(item.DestinationDirection, item.DestinationPosition);
            Vector2 startPos = GetEdgePosition(startPort);
            Vector2 endPos = GetEdgePosition(endPort);
            bool straight = isStraightLine(startPos, endPos);
            float x = 0;
            float y = 0;
            if (straight)
            {
                x = Interpolate(startPos.X, endPos.X, item.Progress);
                y = Interpolate(startPos.Y, endPos.Y, item.Progress);
            }
            else
            {
                Vector2 midPoint = GetMidPoint(startPos, endPos);
                float firstLength = (startPos - midPoint).Length();
                float secondLength = (endPos - midPoint).Length();
                float totalLength = firstLength + secondLength;
                float firstStep = firstLength / totalLength;
                float secondStep = secondLength / totalLength;
                if (item.Progress <= firstStep)
                {
                    x = Interpolate(startPos.X, midPoint.X, Remap((float)item.Progress, 0, firstStep, 0, 1));
                    y = Interpolate(startPos.Y, midPoint.Y, Remap((float)item.Progress, 0, firstStep, 0, 1));
                }
                else
                {
                    x = Interpolate(midPoint.X, endPos.X, Remap((float)item.Progress - firstStep, 0, firstStep, 0, 1));
                    y = Interpolate(midPoint.Y, endPos.Y, Remap((float)item.Progress - firstStep, 0, firstStep, 0, 1));
                }
            }


            return new Vector2(x, y);

        }

        public bool IsPositionValid(ItemLogisticsComponentPort.PortDirection direction, ItemLogisticsComponentPort.PortType type, uint position = 0)
        {
            switch (direction)
            {
                case ItemLogisticsComponentPort.PortDirection.N:
                case ItemLogisticsComponentPort.PortDirection.S:
                    if (rotatedSize.X - 1 < position) return false;
                    break;
                case ItemLogisticsComponentPort.PortDirection.E:
                case ItemLogisticsComponentPort.PortDirection.W:
                    if (rotatedSize.Y - 1 < position) return false;
                    break;
            }
            switch (type)
            {
                case ItemLogisticsComponentPort.PortType.Input:
                    foreach (var input in Inputs)
                    {
                        var rotatedDirection = GetRotatedDirection(input.Direction);
                        var rotatedPosition = GetRotatedPosition(input.Direction, input.Position);
                        if (rotatedDirection == direction && rotatedPosition == position) return false;
                    }
                    break;
                case ItemLogisticsComponentPort.PortType.Output:
                    foreach (var output in Outputs)
                    {
                        var rotatedDirection = GetRotatedDirection(output.Direction);
                        var rotatedPosition = GetRotatedPosition(output.Direction, output.Position);
                        if (rotatedDirection == direction && rotatedPosition == position) return false;
                    }
                    break;
            }
            return true;
        }
        public void AddInput(ItemLogisticsComponentPort.PortDirection direction, uint position = 0)
        {
            if (IsPositionValid(direction, ItemLogisticsComponentPort.PortType.Input, position))
            {
                ItemLogisticsComponentPort port = new ItemLogisticsComponentPort(ItemLogisticsComponentPort.PortType.Input, direction) { Position = position };
                Inputs.Add(port);
            }
        }
        public void AddOutput(ItemLogisticsComponentPort.PortDirection direction, uint position = 0)
        {
            if (IsPositionValid(direction, ItemLogisticsComponentPort.PortType.Input, position))
            {
                ItemLogisticsComponentPort port = new ItemLogisticsComponentPort(ItemLogisticsComponentPort.PortType.Output, direction) { Position = position };
                Outputs.Add(port);
            }

        }

        public ItemLogisticsComponentPort GetInput(ItemLogisticsComponentPort.PortDirection direction, uint position = 0)
        {
            foreach (var input in Inputs)
            {
                // var rotatedDirection = GetRotatedDirection(input.Direction,true);
                // var rotatedPosition = GetRotatedPosition(input.Direction, input.Position);
                if (input.Direction == direction && input.Position == position) return input;
            }
            return null;
        }

        public ItemLogisticsComponentPort GetOutput(ItemLogisticsComponentPort.PortDirection direction, uint position = 0)
        {
            foreach (var output in Outputs)
            {
                //var rotatedDirection = GetRotatedDirection(output.Direction, true);
                //var rotatedPosition = GetRotatedPosition(output.Direction, output.Position);
                if (output.Direction == direction && output.Position == position) return output;
            }
            return null;
        }

        public void AddItemToInput(Item item, ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {

            var portDir = GetRotatedDirection(direction, true);
            uint portPosition = CalculatePortPosition(portDir, position);
            var port = GetInput(portDir, portPosition);

            if (port != null)
            {
                if (IsFull())
                {
                    return;
                }
                LogisticsItem logisticsComponentItem = new LogisticsItem(item);
                logisticsComponentItem.SourcePosition = port.Position;
                logisticsComponentItem.SourceDirection = port.Direction;
                var currentOutputPort = Outputs[currentOutput];
                var outputDir = GetRotatedDirection(currentOutputPort.Direction);
                logisticsComponentItem.DestinationDirection = currentOutputPort.Direction;
                logisticsComponentItem.DestinationPosition = currentOutputPort.Position;//GetRotatedPosition(outputDir, currentOutputPort.Position);
                port.Items.Add(logisticsComponentItem);
            }

        }
        public void AddItemToOutput(Item item, ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {
            uint portPosition = CalculatePortPosition(direction, position);
            var port = GetOutput(direction, portPosition);

            if (port != null)
            {
                if (port.IsFull())
                {
                    return;
                }
                LogisticsItem logisticsComponentItem = new LogisticsItem(item);
                port.Items.Add(logisticsComponentItem);
            }

        }

        public bool IsFull()
        {
            int count = 0;
            foreach (var input in Inputs)
            {
                count += input.Items.Count();
            }
            foreach (var output in Outputs)
            {
                count += output.Items.Count();
            }
            return count >= ItemCountLimit;
        }
        public uint CalculatePortPosition(ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {
            uint portPosition = 0;

            switch (direction)
            {
                case ItemLogisticsComponentPort.PortDirection.N:
                case ItemLogisticsComponentPort.PortDirection.S:
                    portPosition = (uint)(position.X - rotatedSize.X - Position.X + 1);
                    break;
                case ItemLogisticsComponentPort.PortDirection.E:
                case ItemLogisticsComponentPort.PortDirection.W:
                    portPosition = (uint)(position.Y - rotatedSize.Y - Position.Y + 1);
                    break;
            }

            return portPosition;
        }
        public bool CanAcceptItemsFrom(ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {
            uint portPosition = CalculatePortPosition(direction, position);

            foreach (var input in Inputs)
            {
                var rotatedDirection = GetRotatedDirection(input.Direction);
                var rotatedPosition = GetRotatedPosition(input.Direction, input.Position);
                if (rotatedDirection == direction && rotatedPosition == portPosition)
                {
                    if (!IsFull() && CanMove(GetEdgePosition(input)))
                        return true;
                }
            }
            return false;
        }
        public bool CanExportItemsTo(ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {
            uint portPosition = CalculatePortPosition(direction, position);
            foreach (var output in Outputs)
            {
                var rotatedDirection = GetRotatedDirection(output.Direction);
                var rotatedPosition = GetRotatedPosition(output.Direction, output.Position);
                if (rotatedDirection == direction && rotatedPosition == portPosition)
                {

                    if (output.HasItems)
                        return true;
                }
            }
            return false;
        }
        public bool Empty()
        {
            foreach (var input in Inputs)
                if (input.HasItems)
                    return false;
            foreach (var output in Outputs)
                if (output.HasItems)
                    return false;
            return true;
        }

        public float GetClosestItem(Vector2 position)
        {
            float lastDist = float.MaxValue;
            Vector2 result = Vector2.Zero;
            foreach (var input in Inputs)
            {
                if (input.HasItems)
                {
                    foreach (var it in input.Items)
                    {
                        var pos = GetItemPosition(it);
                        float dist = Vector2.Distance(position, pos);
                        if (dist < lastDist)
                        {
                            lastDist = dist;
                            result = pos;
                        }
                    }
                }
            }
            foreach (var output in Outputs)
            {
                if (output.HasItems)
                {
                    foreach (var it in output.Items)
                    {
                        var pos = GetItemPosition(it);
                        float dist = Vector2.Distance(position, pos);
                        if (dist < lastDist)
                        {
                            lastDist = dist;
                            result = pos;
                        }
                    }
                }
            }
            return lastDist;
        }
        public float GetClosestItem(LogisticsItem value)
        {
            float lastDist = float.MaxValue;
            var position = GetItemPosition(value);
            Vector2 result = Vector2.Zero;
            foreach (var input in Inputs)
            {
                if (input.HasItems)
                {
                    foreach (var it in input.Items)
                    {
                        if (it != value && it.Progress > value.Progress)
                        {
                            var pos = GetItemPosition(it);
                            float dist = Vector2.Distance(position, pos);
                            if (dist < lastDist)
                            {
                                lastDist = dist;
                                result = pos;
                            }
                        }
                    }
                }
            }
            foreach (var output in Outputs)
            {
                if (output.HasItems)
                {
                    foreach (var it in output.Items)
                    {
                        if (it != value && it.Progress >= value.Progress)
                        {
                            var pos = GetItemPosition(it);
                            float dist = Vector2.Distance(position, pos);
                            if (dist < lastDist)
                            {
                                lastDist = dist;
                                result = pos;
                            }
                        }
                    }
                }
            }
            return lastDist;
        }
        public bool CanMove(LogisticsItem value)
        {
            if (Empty())
            {
                return true;
            }
            var pos = GetClosestItem(value);
            if (pos == float.MaxValue)
                return true;
            if (pos < .5f)
                return false;
            return true;
        }

        public bool CanMove(Vector2 position)
        {
            if (Empty())
            {
                return true;
            }
            var pos = GetClosestItem(position);
            if (pos == float.MaxValue)
                return true;
            if (pos <  .5f)
                return false;
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double step = gameTime.ElapsedGameTime.TotalMilliseconds / ProcessingTime;
            base.Update(gameTime);
            foreach (var input in Inputs)
            {

                if (input.HasItems)
                {
                    List<LogisticsItem> toRemove = new List<LogisticsItem>();
                    foreach (var item in input.Items)
                    {
                        if (CanMove(item))
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
