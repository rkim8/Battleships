using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rk
{
    class Point : ICloneable
    {
        public int X { get { return x; } }
        public int Y { get { return y; } }
        private int x = 0;
        private int y = 0;

        static Random PointGen = new Random();

        public Point() 
        {
            this.x = 0;
            this.y = 0;
        }

        public Point(int x, int y) 
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point p) 
        {
            this.x = p.X;
            this.y = p.Y;
        }

        public object Clone() 
        {
            return this.MemberwiseClone();
        }

        public static Point CreatePointFrom(Point initialPoint, int length, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(initialPoint.X - length, initialPoint.Y);
                case Direction.South:
                    return new Point(initialPoint.X + length, initialPoint.Y);
                case Direction.West:
                    return new Point(initialPoint.X, initialPoint.Y - length);
                case Direction.East:
                    return new Point(initialPoint.X, initialPoint.Y + length);
                default:
                    return new Point();
            }
        }
        public static Point GetRandomPoint()
        {
            return new Point(PointGen.Next(10),PointGen.Next(10));
        }

        public static bool ValidatePoint(Point p)
        {
            if (p.X < 0) return false;
            else if (p.Y < 0) return false;
            else if (p.X >= 10) return false;
            else if (p.Y >= 10) return false;
            else return true;
        }
    }
}
