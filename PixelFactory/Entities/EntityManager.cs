﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Entities
{
    public class EntityManager
    {
        private List<Entity> entities;
        public Camera Camera { get; set; }
        public EntityManager()
        {
            entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            entities.Add(entity);
            entity.EntityManager = this;
        }
        public void Remove(Entity entity) 
        {
            entity.Dispose();
            entities.Remove(entity);
        }


        public Entity GetFromPosition(Vector2 positon)
        {
            foreach (var entity in entities)
            {
                if (entity is DrawableEntity)
                {
                    var drawable = entity as DrawableEntity;
                    var bounds = new Rectangle(drawable.Position.ToPoint(), drawable.Size.ToPoint());
                    if (Utils.HelperFunctions.IsInBounds(positon, bounds))
                    {
                        return entity;
                    }
                }
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                entity.Update(gameTime);
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
            {
                if (entity is DrawableEntity)
                {
                    var drawable = entity as DrawableEntity;
                    if (Camera.IsInviewport(drawable.Position, drawable.Size))
                    {
                        drawable.Zoom = Camera.Zoom;
                        drawable.Draw(gameTime, spriteBatch);
                    }
                }
            }
        }
    }
}
