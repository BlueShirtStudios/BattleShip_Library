using System;
using System.Collections.Generic;


namespace BattleShipCollection
{
    public class BattleShip
    {
        //Properties
        private string name;
        private int health;
        private int width;
        private int length;
        private List<Coordinate> occupiedCoordinates = new List<Coordinate>();

        public string Name
        {
            get { return this.name; }
        }

        public int Health
        {
            get { return this.health; }
            set { this.health = value; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Length
        {
            get { return this.length; }
        }

       public List<Coordinate> OccupiedCoordinates
        {
            get { return this.occupiedCoordinates; }
            set { this.occupiedCoordinates = value; }
        }

        public BattleShip(string cName, int cWidth, int cLength)
        {
            this.name = cName;
            this.width = cWidth;
            this.length = cLength;
            this.health = cLength * 20;
        }

        public string GenerateShipKey()
        {
            return $"{this.Length}DotShip{Name.Substring(0, 2)}";
        }

        public bool IsInXPos(int x)
        {
            //Checks if a given x coordinate is already occupied
            foreach(Coordinate coord in OccupiedCoordinates)
            {
                if (coord.X == x)
                {
                    return true;
                }
            }
            return false;
           
        }

        public bool IsInYPos(int y)
        {
            //Checks if a given y coordinate is already occupied
            foreach(Coordinate coord in OccupiedCoordinates)
            {
                if (coord.Y == y)
                {
                    return true;
                }
            }

            return false;
        }

        //Test Method here
        public void DisplayShipCoords()
        {
            foreach(Coordinate coord in occupiedCoordinates)
            {
                int x = coord.X;
                int y = coord.Y;
                Console.WriteLine($"{x}, {y}");
            }
        }

        public void TakeDamage(Coordinate coord)
        {
            Health -= 20;
            OccupiedCoordinates.Remove(coord);
        }

    }

}//namespace

