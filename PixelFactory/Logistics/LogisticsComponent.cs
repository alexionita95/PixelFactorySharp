using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using System.Collections.Generic;

namespace PixelFactory.Logistics
{
    public class LogisticsComponent : DrawableEntity
    {

        public float Capacity { get; set; } = 4;
        public float AvailableSpace { get => Capacity - Count(); }
        public List<Port> Ports { get; set; }
        public float ProcessingTime { get; set; }


        public LogisticsComponent(Vector2 size)
       : base(size)
        {
            Ports = new List<Port>();
            Layer = DrawLayer.Buildings;
        }
        public LogisticsComponent()
         : base(Vector2.One)
        {
            Ports = new List<Port>();
            Layer = DrawLayer.Buildings;
        }
        public virtual bool IsFull()
        {
            return Count() == Capacity;
        }
        public virtual bool IsEmpty()
        {
            return Count() == 0;
        }
        public virtual float Count()
        {
            return 0;
        }
        public bool ValidatePositionOnPorts(Direction direction, List<Port> ports, uint position = 0)
        {
            foreach (var port in ports)
            {
                var rotatedDirection = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                var rotatedPosition = DirectionUtils.GetRotatedPosition(port.Direction, port.Position, Rotation, RotatedSize);
                if (rotatedDirection == direction && rotatedPosition == position)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsPositionValid(Direction direction, uint position = 0)
        {
            switch (direction)
            {
                case Direction.N:
                case Direction.S:
                    if (RotatedSize.X - 1 < position) return false;
                    break;
                case Direction.E:
                case Direction.W:
                    if (RotatedSize.Y - 1 < position) return false;
                    break;
            }
            ValidatePositionOnPorts(direction, Ports, position);
            return true;
        }
        public void AddInput(Direction direction, InventoryEntityType entityType = InventoryEntityType.Solid, uint position = 0)
        {
            if (IsPositionValid(direction, position))
            {
                Port port = new Port(Port.PortType.Input,entityType, direction) { Position = position };
                Ports.Add(port);
            }
        }
        public void AddOutput(Direction direction, InventoryEntityType entityType = InventoryEntityType.Solid,uint position = 0)
        {
            if (IsPositionValid(direction, position))
            {
                Port port = new Port(Port.PortType.Output,entityType, direction) { Position = position };
                Ports.Add(port);
            }

        }

        public void AddIO(Direction direction, InventoryEntityType entityType = InventoryEntityType.Solid, uint position = 0)
        {
            if (IsPositionValid(direction, position))
            {
                Port port = new Port(Port.PortType.IO, entityType, direction) { Position = position };
                Ports.Add(port);
            }

        }

        public Port GetInput(Direction direction, uint position = 0)
        {
            foreach (var input in Ports)//Inputs)
            {
                if (input.IsOutput)
                    continue;
                if (input.Direction == direction && input.Position == position) return input;
            }
            return null;
        }
        public Port GetOutput(Direction direction, uint position = 0)
        {
            foreach (var output in Ports)//Outputs)
            {
                if (output.IsInput)
                    continue;
                if (output.Direction == direction && output.Position == position) return output;
            }
            return null;
        }
        public virtual void Add(InventoryEntity entity, Direction direction, Vector2 position)
        {
        }
        public virtual void Add(InventoryEntity entity, float count, Direction direction, Vector2 position)
        {
        }

        public virtual bool CanAcceptFrom(InventoryEntityType entityType, Direction direction, Vector2 position)
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
                        return true;
                }
            }
            return false;
        }
        public virtual bool CanAcceptFrom(InventoryEntityType entityType, float count, Direction direction, Vector2 position)
        {
            return false;
        }
        public virtual bool CanExportTo(InventoryEntityType entityType, Direction direction, Vector2 position)
        {
            return false;
        }

        public virtual bool CanExportTo(InventoryEntityType entityType, float count, Direction direction, Vector2 position)
        {
            return false;
        }



        public int GetValidInputsCount(InventoryEntityType entityType)
        {
            int result = 0;
            foreach (var port in Ports)
            {
                if (port.IsOutput)
                {
                    continue;
                }
                Vector2 pos = DirectionUtils.GetNextPosition(port.Direction, port.Position, Rotation, Position, RotatedSize);
                Entity entity = EntityManager.GetFromPosition(pos);
                if (entity is LogisticsComponent)
                {
                    var component = entity as LogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (component.CanExportTo(entityType,opposite, pos))
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public int GetValidOutputsCount(InventoryEntityType entityType)
        {
            int result = 0;
            foreach (var port in Ports)
            {
                if (port.IsInput)
                {
                    continue;
                }
                Vector2 pos = DirectionUtils.GetNextPosition(port.Direction, port.Position, Rotation, Position, RotatedSize);
                Entity entity = EntityManager.GetFromPosition(pos);
                if (entity is LogisticsComponent)
                {
                    var component = entity as LogisticsComponent;
                    var dir = DirectionUtils.GetRotatedDirection(port.Direction, Rotation);
                    var opposite = DirectionUtils.GetOppositeDirection(dir);
                    if (component.CanAcceptFrom(entityType, opposite, pos))
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public bool ValidateInput(InventoryEntityType entityType, Direction direction)
        {
            foreach (var input in Ports)
            {
                if (input.IsOutput)
                {
                    continue;
                }
                if (input.Direction == direction)
                {
                    Vector2 pos = DirectionUtils.GetNextPosition(input.Direction, input.Position, Rotation, Position, RotatedSize);
                    Entity entity = EntityManager.GetFromPosition(pos);
                    if (entity == null)
                    {
                        return false;
                    }
                    if (entity is LogisticsComponent)
                    {
                        var component = entity as LogisticsComponent;
                        var dir = DirectionUtils.GetRotatedDirection(input.Direction, Rotation);
                        var opposite = DirectionUtils.GetOppositeDirection(dir);
                        if (component.CanExportTo(entityType, opposite, pos))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool ValidateOutput(InventoryEntityType entityType, Direction direction)
        {
            foreach (var output in Ports)
            {
                if (output.IsInput)
                {
                    continue;
                }
                if (output.Direction == direction)
                {
                    Vector2 pos = DirectionUtils.GetNextPosition(output.Direction, output.Position, Rotation, Position, RotatedSize);
                    Entity entity = EntityManager.GetFromPosition(pos);
                    if (entity == null)
                    {
                        return false;
                    }
                    if (entity is LogisticsComponent)
                    {
                        var component = entity as LogisticsComponent;
                        var dir = DirectionUtils.GetRotatedDirection(output.Direction, Rotation);
                        var opposite = DirectionUtils.GetOppositeDirection(dir);
                        if (component.CanAcceptFrom(entityType, opposite, pos))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
}
}
