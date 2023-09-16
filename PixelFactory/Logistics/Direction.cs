using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using static PixelFactory.Entities.DrawableEntity;

namespace PixelFactory.Logistics
{
    public enum Direction
    {
        N, W, S, E

    }
    public class DirectionUtils
    {
        public static Direction IncrementDirection(Direction direction, int increment)
        {
            int length = Enum.GetValues(typeof(Direction)).Length;
            if (increment < 0) { increment += length; }

            return (Direction)(((int)direction + increment) % length);
        }
        public static bool AreOpposite(Direction direction, Direction other)
        {
            if ((direction == Direction.N && other == Direction.S) ||
                (direction == Direction.S && other == Direction.N) ||
                (direction == Direction.E && other == Direction.W) ||
                (direction == Direction.W && other == Direction.E))
            {
                return true;
            }
            return false;
        }
        public static bool IsDirectionBefore(Direction direction, Direction other)
        {
            if (IncrementDirection(direction, 1) == other)
                return true;
            return false;
        }
        public static Direction GetRotatedDirection(Direction direction, EntityRotation rotation, bool back = false)
        {
            int increment = 0;
            switch (rotation)
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
        public static Direction GetOppositeDirection(Direction direction)
        {

            switch (direction)
            {
                case Direction.N:
                    return Direction.S;
                case Direction.W:
                    return Direction.E;
                case Direction.S:
                    return Direction.N;
                case Direction.E:
                    return Direction.W;
            }
            return Direction.N;
        }
        public static uint GetRotatedPosition(Direction direction, uint position, EntityRotation rotation, Vector2 size)
        {
            var rotatedDir = GetRotatedDirection(direction, rotation);
            if (AreOpposite(direction, rotatedDir) || IsDirectionBefore(rotatedDir, direction))
            {
                switch (direction)
                {
                    case Direction.N:
                    case Direction.S:
                        return (uint)size.X - position - 1;
                    case Direction.E:
                    case Direction.W:
                        return (uint)size.Y - position - 1;
                }
            }
            return position;
        }
        public static Vector2 GetEdgePosition(Direction direction, uint position, EntityRotation rotation, Vector2 size)
        {
            var rotatedDirection = GetRotatedDirection(direction, rotation);
            var rotatedPosition = GetRotatedPosition(direction, position, rotation, size);
            var coordinate = rotatedPosition * 0.5f + 0.5f;
            switch (rotatedDirection)
            {
                case Direction.N:
                    return new Vector2(coordinate, 0);
                case Direction.S:
                    return new Vector2(coordinate, size.Y);
                case Direction.E:
                    return new Vector2(0, coordinate);
                case Direction.W:
                    return new Vector2(size.X, coordinate);
            }

            return Vector2.Zero;
        }

        public static Vector2 GetInsideEdgePosition(Direction direction, uint position, EntityRotation rotation, Vector2 size, Vector2 scale)
        {
            var rotatedDirection = GetRotatedDirection(direction, rotation);
            var rotatedPosition = GetRotatedPosition(direction, position, rotation, size);
            var coordinate = rotatedPosition * 0.5f + 0.5f;
            switch (rotatedDirection)
            {
                case Direction.N:
                    return new Vector2(coordinate - scale.X/2, -scale.Y/2);
                case Direction.S:
                    return new Vector2(coordinate -scale.X/2, size.Y - scale.Y/2);
                case Direction.E:
                    return new Vector2(-scale.X/2, coordinate -scale.Y/2);
                case Direction.W:
                    return new Vector2(size.X-scale.X/2, coordinate - scale.Y/2);
            }

            return Vector2.Zero;
        }

        public static uint CalculatePortPosition(Direction direction, Vector2 position, Vector2 origin, Vector2 size)
        {
            uint portPosition = 0;

            switch (direction)
            {
                case Direction.N:
                case Direction.S:
                    portPosition = (uint)(position.X - size.X - origin.X + 1);
                    break;
                case Direction.E:
                case Direction.W:
                    portPosition = (uint)(position.Y - size.Y - origin.Y + 1);
                    break;
            }
            return portPosition;
        }
        public static Vector2 GetNextPosition(Direction direction, uint position, EntityRotation rotation, Vector2 origin, Vector2 size)
        {
            var dir = GetRotatedDirection(direction, rotation);
            Vector2 edgePos = GetEdgePosition(direction, position, rotation, size);
            edgePos += origin;
            switch (dir)
            {
                case Direction.N:
                    return new Vector2((int)edgePos.X, (int)edgePos.Y -1);
                case Direction.S:
                    return new Vector2((int)edgePos.X, (int)edgePos.Y + size.Y - 1);
                case Direction.W:
                    return new Vector2((int)edgePos.X + size.X - 1, (int)edgePos.Y);
                case Direction.E:
                    return new Vector2((int)edgePos.X - 1, (int)edgePos.Y);
            }
            return Vector2.Zero;
        }

    }
}
