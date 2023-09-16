﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PixelFactory.Utils
{
    public class HelperFunctions
    {
        public static float Interpolate(float start, float end, double progress)
        {
            return start + (end - start) * (float)progress;
        }
        public static bool IsStraightLine(Vector2 start, Vector2 end)
        {
            if (start.X == end.X || start.Y == end.Y) return true;
            return false;
        }

        public static bool IsOnEdge(float value, float min = 0, float max = 1)
        {
            if (value == min || value == max) return true;
            return false;
        }
        public static Vector2 GetMidPoint(Vector2 start, Vector2 end, Vector2 size, Vector2 scale)
        {
            float midX = 0;
            float midY = 0;

            if (IsOnEdge(start.X, -scale.X, size.X - scale.X))
                midX = end.X;
            else
                midX = start.X;
            if (IsOnEdge(start.Y, -scale.Y, size.Y - scale.Y))
                midY = end.Y;
            else
                midY = start.Y;

            return new Vector2(midX, midY);
        }
        public static float Remap(float value, float min1, float max1, float min2, float max2)
        {
            return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
        }

    }
}