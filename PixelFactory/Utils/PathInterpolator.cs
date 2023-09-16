using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using PixelFactory.Items;
using System.Net;

namespace PixelFactory.Utils
{
    public class PathInterpolator
    {
        public List<Vector2> Points { get; private set; }
        public PathInterpolator()
        {
            Points = new List<Vector2>();
        }
        public void AddPoint(Vector2 point)
        {
            Points.Add(point);
        }
        private float GetTotalLength()
        {
            float length = 0;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                var localLength = (Points[i] - Points[i + 1]).Length();
                length += localLength;
            }
            return length;
        }
        public Vector2 Interpolate(double progress)
        {
            float localLength = 0;
            float lastLength;
            float totalLenght = GetTotalLength();

            for (int i = 0; i < Points.Count - 1; i++)
            {
                float length = (Points[i] - Points[i + 1]).Length();
                lastLength = localLength;
                float lastProgress = lastLength != 0 ? lastLength / totalLenght : 0;
                localLength += length;
                float lengthProgress = localLength / totalLenght;
                if (progress <= lengthProgress)
                {
                    double localProgress = progress - lastProgress;
                    float interpolationStep = HelperFunctions.Remap((float)progress, lastProgress, lengthProgress, 0, 1);
                    float x = HelperFunctions.Interpolate(Points[i].X, Points[i + 1].X, interpolationStep);
                    float y = HelperFunctions.Interpolate(Points[i].Y, Points[i + 1].Y, interpolationStep);
                    return new Vector2(x, y);
                }
            }
            return Vector2.Zero;
        }

    }
}
