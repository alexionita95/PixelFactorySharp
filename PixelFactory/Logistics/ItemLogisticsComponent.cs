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


            return (ItemLogisticsComponentPort.PortDirection)(((int)direction + increment) % length);
        }
        public ItemLogisticsComponentPort.PortDirection GetRotatedDirection(ItemLogisticsComponentPort.PortDirection direction)
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
            return IncrementDirection(direction, increment);
           
        }
        private uint GetRotatedPosition(ItemLogisticsComponentPort.PortDirection direction, uint position)
        {
            var rotatedDir = GetRotatedDirection(direction);
            if(AreOpposite(direction,rotatedDir) || IsDirectionBefore(rotatedDir, direction))
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

        public bool IsPositionValid(ItemLogisticsComponentPort.PortDirection direction, ItemLogisticsComponentPort.PortType type, uint position = 0)
        {
            switch (direction)
            {
                case ItemLogisticsComponentPort.PortDirection.N:
                case ItemLogisticsComponentPort.PortDirection.S:
                    if(rotatedSize.X - 1 < position) return false;
                    break;
                case ItemLogisticsComponentPort.PortDirection.E:
                case ItemLogisticsComponentPort.PortDirection.W:
                    if (rotatedSize.Y - 1 < position) return false;
                    break;
            }
            switch (type)
            {
                case ItemLogisticsComponentPort.PortType.Input:
                    foreach(var input in Inputs)
                    {
                        var rotatedDirection = GetRotatedDirection(input.Direction);
                        var rotatedPosition = GetRotatedPosition(input.Direction, input.Position);
                        if (rotatedDirection== direction && rotatedPosition == position) return false;
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
            foreach(var input in Inputs)
            {
                var rotatedDirection = GetRotatedDirection(input.Direction);
                var rotatedPosition = GetRotatedPosition(input.Direction, input.Position);
                if (rotatedDirection== direction && rotatedPosition == position) return input;
            }
            return null;
        }

        public ItemLogisticsComponentPort GetOutput(ItemLogisticsComponentPort.PortDirection direction, uint position = 0)
        {
            foreach (var output in Outputs)
            {
                var rotatedDirection = GetRotatedDirection(output.Direction);
                var rotatedPosition = GetRotatedPosition(output.Direction, output.Position);
                if (rotatedDirection == direction && rotatedPosition == position) return output;
            }
            return null;
        }

        public void AddItemToInput(Item item, ItemLogisticsComponentPort.PortDirection direction, Vector2 position)
        {
            uint portPosition = CalculatePortPosition(direction, position);
            var port = GetInput(direction, portPosition);
  
            if(port != null)
            {
                if (IsFull())
                {
                    return;
                }
                LogisticsComponentItem logisticsComponentItem = new LogisticsComponentItem(item);
                logisticsComponentItem.SourcePosition = portPosition;
                logisticsComponentItem.SourceDirection = direction;
                var currentOutputPort = Outputs[currentOutput];
                logisticsComponentItem.DestinationDirection = currentOutputPort.Direction;
                logisticsComponentItem.DestinationPosition = currentOutputPort.Position;
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
                LogisticsComponentItem logisticsComponentItem = new LogisticsComponentItem(item);
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
                    portPosition = (uint)(position.X - rotatedSize.X);
                    break;
                case ItemLogisticsComponentPort.PortDirection.E:
                case ItemLogisticsComponentPort.PortDirection.W:
                    portPosition = (uint)(position.Y - rotatedSize.Y);
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
                    if (!IsFull())
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
