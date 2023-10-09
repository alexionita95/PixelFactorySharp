using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Logistics.Fluids
{
    public class Pipe : FluidLogisticsComponent
    {
        public enum JunctionType { None, Corner, T, Cross }
        JunctionType junctionType;
        public Pipe()
        {
            AddIO(Direction.N, InventoryEntityType.Fluid, 0);
            AddIO(Direction.E, InventoryEntityType.Fluid, 0);
            AddIO(Direction.W, InventoryEntityType.Fluid, 0);
            AddIO(Direction.S, InventoryEntityType.Fluid, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Effects = SpriteEffects.None;
            junctionType = JunctionType.None;
            Texture.Rotation = Rotation;
            InventoryEntityType type = InventoryEntityType.Fluid;
            var connectedEntities = GetConnectedEntites();
            int validPortsCount = connectedEntities.Count;
            switch (validPortsCount)
            {
                case 1:
                    Direction direction = connectedEntities.First().Key;
                    EntityRotation rotation = connectedEntities.First().Value.Rotation;
                    if (Utils.HelperFunctions.IsPerpendicular(Rotation, rotation))
                    {
                        junctionType = JunctionType.Corner;
                        switch (Rotation)
                        {
                            case EntityRotation.None:
                                if (direction == Direction.N || direction == Direction.S)
                                {
                                    junctionType = JunctionType.None;
                                }
                                if (direction == Direction.W)
                                {
                                    Texture.Rotation = EntityRotation.Rot270;
                                }
                                if (direction == Direction.E)
                                {
                                    Texture.Rotation = EntityRotation.None;
                                }
                                break;
                            case EntityRotation.Rot90:
                                if (direction == Direction.W || direction == Direction.E)
                                {
                                    junctionType = JunctionType.None;
                                }
                                if (direction == Direction.N)
                                {
                                    Texture.Rotation = EntityRotation.Rot90;
                                }
                                if (direction == Direction.S)
                                {
                                    Texture.Rotation = EntityRotation.None;
                                }
                                break;
                            case EntityRotation.Rot180:
                                if (direction == Direction.N || direction == Direction.S)
                                {
                                    junctionType = JunctionType.None;
                                }
                                if (direction == Direction.W)
                                {
                                    Texture.Rotation = EntityRotation.Rot180;
                                }
                                if (direction == Direction.E)
                                {
                                    Texture.Rotation = EntityRotation.Rot90;
                                }
                                break;
                            case EntityRotation.Rot270:
                                if (direction == Direction.W || direction == Direction.E)
                                {
                                    junctionType = JunctionType.None;
                                }

                                if (direction == Direction.N)
                                {
                                    Texture.Rotation = EntityRotation.Rot180;
                                }
                                if (direction == Direction.S)
                                {
                                    Texture.Rotation = EntityRotation.Rot270;
                                }
                                break;
                        }
                    }
                    break;
                case 2:
                    junctionType = JunctionType.None;
                    EntityRotation firstRotation = connectedEntities.ElementAt(0).Value.Rotation;
                    EntityRotation secondRotation = connectedEntities.ElementAt(1).Value.Rotation;
                    if (Utils.HelperFunctions.IsPerpendicular(Rotation, firstRotation) && Utils.HelperFunctions.IsPerpendicular(Rotation, secondRotation))
                    {
                        junctionType = JunctionType.T;
                    }
                    else
                    {
                        if (Utils.HelperFunctions.IsPerpendicular(Rotation, firstRotation))
                        {
                            direction = connectedEntities.ElementAt(0).Key;
                            switch (Rotation)
                            {
                                case EntityRotation.None:
                                    if (direction == Direction.N || direction == Direction.S)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.W)
                                    {
                                        Texture.Rotation = EntityRotation.Rot270;
                                    }
                                    if (direction == Direction.E)
                                    {
                                        Texture.Rotation = EntityRotation.None;
                                    }
                                    break;
                                case EntityRotation.Rot90:
                                    if (direction == Direction.W || direction == Direction.E)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.N)
                                    {
                                        Texture.Rotation = EntityRotation.Rot90;
                                    }
                                    if (direction == Direction.S)
                                    {
                                        Texture.Rotation = EntityRotation.None;
                                    }
                                    break;
                                case EntityRotation.Rot180:
                                    if (direction == Direction.N || direction == Direction.S)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.W)
                                    {
                                        Texture.Rotation = EntityRotation.Rot180;
                                    }
                                    if (direction == Direction.E)
                                    {
                                        Texture.Rotation = EntityRotation.Rot90;
                                    }
                                    break;
                                case EntityRotation.Rot270:
                                    if (direction == Direction.W || direction == Direction.E)
                                    {
                                        junctionType = JunctionType.None;
                                    }

                                    if (direction == Direction.N)
                                    {
                                        Texture.Rotation = EntityRotation.Rot180;
                                    }
                                    if (direction == Direction.S)
                                    {
                                        Texture.Rotation = EntityRotation.Rot270;
                                    }
                                    break;
                            }
                        }
                        if (Utils.HelperFunctions.IsPerpendicular(Rotation, secondRotation))
                        {
                            direction = connectedEntities.ElementAt(1).Key;
                            switch (Rotation)
                            {
                                case EntityRotation.None:
                                    if (direction == Direction.N || direction == Direction.S)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.W)
                                    {
                                        Texture.Rotation = EntityRotation.Rot270;
                                    }
                                    if (direction == Direction.E)
                                    {
                                        Texture.Rotation = EntityRotation.None;
                                    }
                                    break;
                                case EntityRotation.Rot90:
                                    if (direction == Direction.W || direction == Direction.E)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.N)
                                    {
                                        Texture.Rotation = EntityRotation.Rot90;
                                    }
                                    if (direction == Direction.S)
                                    {
                                        Texture.Rotation = EntityRotation.None;
                                    }
                                    break;
                                case EntityRotation.Rot180:
                                    if (direction == Direction.N || direction == Direction.S)
                                    {
                                        junctionType = JunctionType.None;
                                    }
                                    if (direction == Direction.W)
                                    {
                                        Texture.Rotation = EntityRotation.Rot180;
                                    }
                                    if (direction == Direction.E)
                                    {
                                        Texture.Rotation = EntityRotation.Rot90;
                                    }
                                    break;
                                case EntityRotation.Rot270:
                                    if (direction == Direction.W || direction == Direction.E)
                                    {
                                        junctionType = JunctionType.None;
                                    }

                                    if (direction == Direction.N)
                                    {
                                        Texture.Rotation = EntityRotation.Rot180;
                                    }
                                    if (direction == Direction.S)
                                    {
                                        Texture.Rotation = EntityRotation.Rot270;
                                    }
                                    break;
                            }
                        }
                    }

                    switch (Rotation)
                    {
                        case EntityRotation.None:
                            break;
                        case EntityRotation.Rot90:
                            if (junctionType == JunctionType.T)
                            {
                                Texture.Rotation = EntityRotation.Rot90;
                            }
                            break;
                        case EntityRotation.Rot180:
                            break;
                        case EntityRotation.Rot270:
                            if (junctionType == JunctionType.T)
                            {
                                Texture.Rotation = EntityRotation.Rot270;
                            }
                            break;
                    }
                    break;
                case 3:

                    junctionType = JunctionType.T;
                    if (connectedEntities.ContainsKey(Direction.S) && !connectedEntities.ContainsKey(Direction.N))
                    {
                        Texture.Rotation = EntityRotation.None;
                        if (Rotation == EntityRotation.None || Rotation == EntityRotation.Rot180)
                        {
                            junctionType = JunctionType.Cross;
                        }
                    }
                    if (connectedEntities.ContainsKey(Direction.N) && !connectedEntities.ContainsKey(Direction.S))
                    {
                        Texture.Rotation = EntityRotation.Rot180;
                        if (Rotation == EntityRotation.None || Rotation == EntityRotation.Rot180)
                        {
                            junctionType = JunctionType.Cross;
                        }
                    }
                    if (connectedEntities.ContainsKey(Direction.E) && !connectedEntities.ContainsKey(Direction.W))
                    {
                        Texture.Rotation = EntityRotation.Rot90;
                        if (Rotation == EntityRotation.Rot90 || Rotation == EntityRotation.Rot270)
                        {
                            junctionType = JunctionType.Cross;
                        }
                    }
                    if (connectedEntities.ContainsKey(Direction.W) && !connectedEntities.ContainsKey(Direction.E))
                    {
                        if (Rotation == EntityRotation.Rot90 || Rotation == EntityRotation.Rot270)
                        {
                            junctionType = JunctionType.Cross;
                        }
                        Texture.Rotation = EntityRotation.Rot270;
                    }
                    break;
                case 4:
                    junctionType = JunctionType.Cross;
                    break;
            }

            switch (junctionType)
            {
                case JunctionType.None:
                    Texture.CurrentRow = 0;
                    break;
                case JunctionType.Corner:
                    Texture.CurrentRow = 1;
                    break;
                case JunctionType.T:
                    Texture.CurrentRow = 2;
                    break;
                case JunctionType.Cross:
                    Texture.CurrentRow = 3;
                    break;
            }
            base.Draw(gameTime, spriteBatch);
        }
    }
}
