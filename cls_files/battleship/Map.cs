using BattleShipCollection;
using System;
using System.Runtime.InteropServices;

namespace BattleShipCollection
{
    public class Map
    {
        //Map Specs
        private int xSize;
        private int ySize;

        //Plot Machine
        private PointPlotter Plotter = default;

        //Ship Registry
        private List<BattleShip> createdShips = new List<BattleShip>();
        private List<BattleShip> activeShips = new List<BattleShip>();

        //Shot Tracker
        private List<Coordinate> missedShots = new List<Coordinate>();
        private List<Coordinate> hitShots = new List<Coordinate>();

        public int XSize
        {
            get { return this.xSize; }
        }

        public int YSize
        {
            get { return this.ySize; }
        }

        public List<BattleShip> ActiveShips
        {
            get { return this.activeShips; }
            set { this.activeShips = value; }
        }

        public List<BattleShip> CreatedShips
        {
            get { return this.createdShips; }
            set { this.createdShips = value; }
        }

        public List<Coordinate> MissedShots
        {
            get { return this.missedShots; }
        }

        public List<Coordinate> HitShots
        {
            get { return this.hitShots; }
        }

        public Map(int cXSize, int cYSize)
        {
            //this.xSize = cXSize;
           // this.ySize = cYSize;
            Plotter = new(cXSize, cYSize);
        }

        private void AddShipToActiveregister(BattleShip Ship)
        {
            activeShips.Add(Ship);
        }

        public void AddShip(BattleShip Ship)
        {
            createdShips.Add(Ship);
        }

        public void PlotShips()
        {
            try
            {
                //Plot each ship that is created
                foreach (BattleShip ShipWhoNeedsCoords in createdShips)
                {
                    //Create the ship's coordinate's set
                    Plotter.CreateShipCoordinateSet(ShipWhoNeedsCoords.Length);

                    //Assign the set to the ship
                    ShipWhoNeedsCoords.OccupiedCoordinates = Plotter.CoordinateSet;

                    //Add ship to active register
                    AddShipToActiveregister(ShipWhoNeedsCoords);

                    //Clean our plotter for the next ship
                    Plotter.ResetPlotter();
                }
            }
            catch (Exception e)
            {

            }
            

        }//PlotShips()

        private bool IsActiveRegistryEmpty()
        {
            if (activeShips.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public BattleShip DoesShipHaveCoordinate(Coordinate requetedCoord)
        {
            //Go through all the ships in the register
            foreach (BattleShip ship in activeShips)
            {
                //Checks if a ship occupies the coordinate
                if (!ScanCoordinateSet(requetedCoord, ship.OccupiedCoordinates))
                {
                    //If it does not find a match
                    continue;
                }

                //If a match is found
                return ship;
            }

            //If no matches were found returns null
            return null;
        }

        private bool ScanCoordinateSet(Coordinate target, List<Coordinate> coordinateSet)
        {
            foreach(Coordinate c in coordinateSet)
            {
                if ((target.X == c.X) && (target.Y == c.Y))
                {
                    return true;
                }
            }

            return false;
        }

        public enum Directions
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

    }
}

