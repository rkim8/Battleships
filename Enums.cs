using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rk
{
    enum Boat
    {
        PatrolBoat,
        Destroyer,
        Submarine,
        Battleship,
        AircraftCarrier,
        None
    }

    enum Player
    {
        Player,
        CPU,
        None
    }

    enum State
    {
        Sunk,
        Floating,
        None
    }
    enum Direction
    {
        West,
        North,
        East,
        South,
        None
    }
    class EnumHelper
    {
        public static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }


        public static string ToString(object value)
        {
            return Regex.Replace(Enum.GetName(value.GetType(),value), @"(?<a>(?<!^)((?:[A-Z][a-z])|(?:(?<!^[A-Z]+)[A-Z0-9]+(?:(?=[A-Z][a-z])|$))|(?:[0-9]+)))", @" ${a}");
        }
    }
}
