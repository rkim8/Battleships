using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace rk
{
    
    class BoatGenerator
    {

        Random boatGen = new Random();

        List<Point[]> boats = new List<Point[]>();

        List<Direction> allowedDirections = null;

        //List<Point> availablePoints = new List<Point>();

    

        public List<Point[]> CalculateBoats()
        {
            for (int i = 0; i < 5; i++)
            {
                boats.Add(CalculateBoat(i));
            }
            return boats;
        }


        private Point[] CalculateBoat(int length)
        {

            do
            {
                allowedDirections = new Direction[] { Direction.West, Direction.North, Direction.East, Direction.South}.ToList() ;
                List<Point> points = new List<Point>();
                
                Point initialPoint = new Point();
                do
	            {
                    initialPoint = new Point(boatGen.Next(0, 10), boatGen.Next(0, 10));
	            } while (!ValidateFullPoint(initialPoint));
                points.Add(initialPoint);
                if (length == 0)
                {
                    return points.ToArray();
                }
                Direction direction = allowedDirections[boatGen.Next(0,allowedDirections.Count)];
                for (int j = 0; j < allowedDirections.Count; j++)
                {
                    Point tempPoint = Point.CreatePointFrom(initialPoint, length, direction);

                    if (ValidateFullPoint(tempPoint))
                    {
                        //availablePoints.Remove(tempPoint);
                        points.Add(tempPoint);
                        break;
                    }
                    allowedDirections.Remove(direction);
                    direction = allowedDirections[boatGen.Next(allowedDirections.Count)];
                   
                }
                if (points.Count != 2) continue;

                Point[] add = null;
                if ((add = CreateRestOfPoints(initialPoint, length, direction)) == null) 
                {
                    continue;
                }
                points.InsertRange(1, add);
                if (points.Count() == (length + 1))
                {
                    return points.ToArray();
                }
            } while (true);
        }

        

        private Point[] CreateRestOfPoints(Point initialPoint, int length, Direction direction) 
        {
            length = length - 1;
            Point[] points = new Point[length];

            for (int j = 0; j < length; j++)
            {
                Point tempPoint = Point.CreatePointFrom(initialPoint, j + 1, direction);

                if (ValidateFullPoint(tempPoint))
                {
                    points[j] = tempPoint;
                    //availablePoints.Remove(tempPoint);
                }
                else 
                {
                    return null;
                }
            }
            return points;
        }

        private bool ValidateFullPoint(Point finalPoint)
        {
            if (!Point.ValidatePoint(finalPoint)) return false;
            foreach (Point[] pArr in boats)
            {
                foreach (Point p in pArr)
                {
                    if( (p.X == finalPoint.X && p.Y == finalPoint.Y) || CheckSurroundings(finalPoint, p) ) return false;
                }
            }
            //if (!availablePoints.Contains(finalPoint)) return false;
            

            return true;
        }
        private bool CheckSurroundings(Point p, Point arrPoint) 
        {
            Point[] surroundings = { 
                                       new Point(p.X - 1, p.Y),
                                       new Point(p.X, p.Y - 1), new Point(p.X, p.Y + 1),
                                       new Point(p.X + 1, p.Y)
                                   };
            foreach (Point check in surroundings)
            {
                if (check.X == arrPoint.X && check.Y == arrPoint.Y )
                {
                    return true;
                }
            }
            return false;
        }

        

        
    }
}
