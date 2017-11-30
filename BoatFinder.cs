using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace rk
{
    class BoatFinder
    {
        static List<BoatFinder> locatedBoats = new List<BoatFinder>();
        static List<Point> usedPoints = new List<Point>();
        static Direction[] directions = new Direction[] { Direction.East, Direction.North, Direction.West, Direction.South };
        static List<Point> availablePoints = new List<Point>();
        static Random pointGen = new Random();
        static List<Boat> boatsSunk = new List<Boat>();
        

        public Point initialPoint = null;
        public Cell cellPoint = null;
        private Random directionGen = new Random();
        private Direction direction = Direction.None;
        private List<Direction> availableDirections = directions.ToList();

        private List<Point> foundPoints = new List<Point>(); 
        
        private bool directionFound = false;

        private int boatDistance = 2;

        public BoatFinder(Point initialPoint, Cell cellFound) 
        {
            this.initialPoint = initialPoint;
            this.cellPoint = cellFound;
            foundPoints.Add(initialPoint);
        }
            
        public static Point GetPoint() 
        {
            if (locatedBoats.Count == 0)
            {
                //return new Point(5, 0);
                return GetRandomPoint();
            }
            else
            {
                return locatedBoats[0].GetChosenPoint();
            }
        }

        private static bool CheckUsed(Point p) 
        {
            return usedPoints.Count(x => x.X == p.X && x.Y == p.Y) > 0;
        }

        private static void InitializeAvailablePoints() 
        {
            int offset = (new Random()).Next(0,1);
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == offset)
                {
                    for (int j = 1; j < 10; j += 2)
                    {
                        availablePoints.Add(new Point(j, i));
                    }
                }
                else
                {
                    for (int j = 0; j < 10; j += 2)
                    {
                        availablePoints.Add(new Point(j, i));
                    }
                }
            }
        }

        private static Point GetRandomPoint() 
        {
            if (availablePoints.Count() == 0) InitializeAvailablePoints();
            Point p;
            do
            {
     
                if (availablePoints.Count() > 0 && !(boatsSunk.Count == 4 && !boatsSunk.Contains(Boat.PatrolBoat)))
                {
                    p = availablePoints[pointGen.Next(0, availablePoints.Count())];
                    availablePoints.Remove(p);
                }
                else
                {
                    p = Point.GetRandomPoint();
                }
            } while (!Point.ValidatePoint(p) && CheckUsed(p));

            usedPoints.Add(p);
            return p;
        }

        public static void BoatFound(Point pointFound,Cell cellFound) 
        {
            if (locatedBoats.Count(x => x.cellPoint.BoatType == cellFound.BoatType) == 0)
                locatedBoats.Add(new BoatFinder(pointFound, cellFound));
            else
                locatedBoats.Where(x => x.cellPoint.BoatType == cellFound.BoatType).First().PointFound(pointFound,cellFound);
        }

        public static void BoatSunk(Cell cellSunk) 
        {
            BoatFinder bf = locatedBoats.Where(x => x.cellPoint.BoatType == cellSunk.BoatType).First();
            bf.PointsSunk();
            locatedBoats.Remove(bf);
            boatsSunk.Add(cellSunk.BoatType);
        }

        public static void BoatNotFound(Cell cellFound)
        {
            if (locatedBoats.Count > 0){
                locatedBoats[0].PointNotFound();
            }
        }

        public Point GetChosenPoint() 
        {
            if (!this.directionFound)
            {
                Point p = this.GetDirection();
                return p;
            }
            else
            {
                Point p;
                bool quit = true;
                do
                {
                    p = this.GetContinuedPoint();
                    quit = true;
                    
                    if (CheckUsed(p))
                    {
                        if (Point.ValidatePoint(p) && Cell.GetCell(p).BoatType == Boat.None)
                        {
                            this.PointNotFound();
                        }
                        quit = false;
                    }
                    else if (!Point.ValidatePoint(p))
                    {
                        this.PointNotFound();
                        quit = false;
                    }
                } while (!quit);
                usedPoints.Add(p);
                return p;
            }
        }

        private Point GetDirection() 
        {
            Point p;
            do
            {
                direction = availableDirections[directionGen.Next(availableDirections.Count)];
                availableDirections.Remove(direction);
                p = Point.CreatePointFrom(this.initialPoint, 1, direction);
            } while (!Point.ValidatePoint(p) || CheckUsed(p));
            usedPoints.Add(p);
            return p;
        }

        private Point GetContinuedPoint() 
        {

            Point p = Point.CreatePointFrom(this.initialPoint, boatDistance, direction);
            boatDistance++;
            return p;
        }

        public void PointFound(Point pointFound, Cell cellFound) 
        {
            if (!directionFound) directionFound = true;
            foundPoints.Add(pointFound);
        }

        public void PointsSunk() 
        {
            foreach (Point p in foundPoints)
            {
                Point[] surroundings = { new Point(p.X - 1, p.Y), new Point(p.X, p.Y - 1), new Point(p.X, p.Y + 1), new Point(p.X + 1, p.Y) };
                foreach (Point check in surroundings)
                {
                    if (!Point.ValidatePoint(check)) continue;
                    if (!usedPoints.Contains(check))usedPoints.Add(check);
                }
            }
        }

        public void PointNotFound()
        {
            if (directionFound)
            {
                direction = directions[(directions.ToList().IndexOf(direction) + 2) % 4];
                boatDistance = 1;
            }
        }
    }
}
