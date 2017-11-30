using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rk
{
    
    class Cell
    {
        public Boat BoatType = Boat.None;
        public Player BoatOwner = Player.None;
        public State BoatState = State.None;
        public Point Location = new Point(-1, -1);

        private static Cell[,] cells = new Cell[10,10];

        public Cell(Player Owner, Point location)
        {
            BoatOwner = Owner;
            Location = location;
            cells[location.X, location.Y] = this;
        }

        public Cell(Player Owner,Point location, Boat Type, State CurrentState)
        {
            BoatType = Type;
            BoatOwner = Owner;
            BoatState = CurrentState;
            Location = location;
            cells[location.X, location.Y] = this;

        }

        public static Cell GetCell(Point p)
        {
            return cells[p.X,p.Y];
        }

    }
}
