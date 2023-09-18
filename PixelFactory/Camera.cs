using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace PixelFactory
{
    public class Camera
    {
        private Vector2 position;
        private Vector2 viewportSize;
        private float zoom = 1.0f;
        public float Zoom { get=>zoom; set { zoom = value; CalculateMatrix(); } }
        public Vector2 Position { get => position; set { position = value; CalculateMatrix(); } }
        public Vector2 ViewportSize { get => viewportSize; set { viewportSize = value; CalculateMatrix(); } }

        public Matrix TransformMatrix { get;private set; }
        public Camera() 
        { 
        }
        public Camera(Vector2 position, Vector2 viwportSize)
        {
            this.Position = position;
            this.ViewportSize = viwportSize;
        }
        private void CalculateMatrix()
        {
            Matrix pos = Matrix.CreateTranslation(-position.X - viewportSize.X/2, -position.Y - viewportSize.Y/2, 0);
            Matrix offset = Matrix.CreateTranslation(ViewportSize.X/2,ViewportSize.Y/2,0);
            TransformMatrix = pos * offset;
        }
        public Vector2 ScreenToWorld(Vector2 position)
        {
            Matrix inverseTransform = Matrix.Invert(TransformMatrix);
            return Vector2.Transform(position, inverseTransform);
        }

        public bool IsInviewport(Vector2 position)
        {
            Rectangle viwport = new Rectangle(ScreenToWorld(new Vector2(Position.X, Position.Y)).ToPoint(), viewportSize.ToPoint());
            if (viwport.Contains(position.ToPoint()))
            {
                return true;
            }
            return false;
        }
        public bool IsInviewport(Vector2 position, Vector2 size)
        {
            Rectangle viwport = new Rectangle(new Vector2(Position.X, Position.Y).ToPoint(),viewportSize.ToPoint());
            Vector2 objPos = Map.MapToScreen(position.X, position.Y)*Zoom;
            Vector2 objSize = Map.MapToScreen(size.X, size.Y)*Zoom;
            Rectangle obj = new Rectangle(objPos.ToPoint(),objSize.ToPoint());
            if(viwport.Contains(obj) || obj.Intersects(viwport)) {
                return true;
            }
            return false;
        }
    }
}
