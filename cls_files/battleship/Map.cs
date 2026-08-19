using BattleEnumCollection;
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
            this.xSize = cXSize;
            this.ySize = cYSize;
            Plotter = new(cXSize, cYSize);
        }

        private void AddShipToActiveregister(BattleShip Ship)
        {
            activeShips.Add(Ship);
        }

        public void PlotShips(List<BattleShip> shipList)
        {
            try
            {
                //Plot each ship that is created
                foreach (BattleShip ShipWhoNeedsCoords in shipList)
                {
                    //Create the ship's coordinate's set
                    Plotter.CreateShipCoordinateSet(ShipWhoNeedsCoords.Length);

                    //Assign the set to the ship
                    ShipWhoNeedsCoords.OccupiedCoordinates = new List<Coordinate>(Plotter.CoordinateSet);

                    //Add ship to active register
                    AddShipToActiveregister(ShipWhoNeedsCoords);

                    //Clean our plotter for the next ship
                    Plotter.ResetPlotter();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            

        }//PlotShips()

        public BattleShip DoesShipHaveCoordinate(Coordinate requetedCoord)
        {
            bool foundMatch = false;

            //Go through all the ships in the register
            foreach (BattleShip ship in activeShips)
            {
                //Checks if a ship occupies the coordinate
                if (!ScanCoordinateSet(requetedCoord, ship.OccupiedCoordinates))
                {
                    //If it does not find a match
                    continue;
                }
                else
                {
                    foundMatch = true;
                }

                //Checks if there was a match found
                //If it was found
                if (foundMatch == true)
                {
                    AddShotToHistory(requetedCoord, foundMatch);
                    return ship;
                }
                
            }

            //Else if it gets here, nothing is returned
            AddShotToHistory(requetedCoord, foundMatch);
            return null;
        }

        private bool ScanCoordinateSet(Coordinate target, List<Coordinate> coordinateSet)
        {
            foreach(Coordinate c in coordinateSet)
            {
                if ((target.X == c.X) && (target.Y == c.Y))
                {
                    //If the pair exists
                    return true;
                }
            }

            //If not
            return false;
        }

        public ShotOutcome[,] BuiltMapRepresentation()
        {
            ShotOutcome[,] mapDisplay = new ShotOutcome[xSize, YSize];
            ShotOutcome state = ShotOutcome.WATER;

            //Build the map
            //For each row in the map
            for (int r = 0; r < xSize; r++)
            {
                //For each colom in map
                for (int c = 0; c < ySize; c++)
                {
                    state = DetermineCoordinateState(r + 1, c + 1); //DEV_NOTE: +1 because coordinate 1 -> Max, not 0 -> Max: 0 will give an error
                    mapDisplay[r, c] = state;
                }
            }

            return mapDisplay;
        }

        private ShotOutcome DetermineCoordinateState(int x, int y)
        {
            ShotOutcome state = ShotOutcome.WATER;
            //Check the hit list
            if (IsInCoordinateList(HitShots, x, y))
            {
                state = ShotOutcome.HIT;
            }

            //Checks the miss list
            else if (IsInCoordinateList(MissedShots, x, y))
            {
                state = ShotOutcome.MISS;
            }

            //Return the state it determined
            return state;
        }

        private bool IsInCoordinateList(List<Coordinate> list, int x, int y)
        {
            //Searches for a coordinate in the provideded list
            foreach (Coordinate c in list)
            {
                if ((c.X == x) && (c.Y == y))
                {
                    //If the coordinate is found in given list
                    return true;
                }
            }

            //Else if not, returns false
            return false;
        }
    
        private void AddShotToHistory(Coordinate passedCoordinate, bool doesShipHaveIt)
        {
            //If the passed coordinate was found, add to hit shot registered
            if (doesShipHaveIt == true)
            {
                HitShots.Add(passedCoordinate);
            }
            else
            {
                missedShots.Add(passedCoordinate);
            }
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

