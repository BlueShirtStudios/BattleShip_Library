using BattleShipCollection;
using System;
using System.Runtime.InteropServices;

namespace BattleShipCollection
{
    public class Map
    {
        private int xSize;
        private int ySize;
        private string shotResult;
        private List<BattleShip> createdShips = new List<BattleShip>();
        private Dictionary<string, BattleShip> activeShips = new Dictionary<string, BattleShip>();
        private List<Coordinate> missedShots = new List<Coordinate>();
        private List<Coordinate> hitShots = new List<Coordinate>();
        private string owner;

        public int XSize
        {
            get { return this.xSize; }
        }

        public int YSize
        {
            get { return this.ySize; }
        }

        public string ShotResult
        {
            get { return this.shotResult; }
            set { this.shotResult = value; }
        }

        public Dictionary<string, BattleShip> ActiveShips
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

        public string Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }


        public Map(int cXSize, int cYSize, string cOwner)
        {
            this.xSize = cXSize;
            this.ySize = cYSize;
            this.owner = cOwner;
        }

        private void AddShipToActiveregister(BattleShip Ship)
        {
            activeShips[Ship.GenerateShipKey()] = Ship;
        }

        public void AddShip(BattleShip Ship)
        {
            createdShips.Add(Ship);
        }

        public void PlotShips()
        {
            //Plot each ship that is created
            foreach (BattleShip ShipWhoNeedsCoords in createdShips)
            {
                Coordinate startPoint = DetermineStartPoint();
                PlotRestOfTheShip(ShipWhoNeedsCoords, startPoint);
                AddShipToActiveregister(ShipWhoNeedsCoords);
            }

        }//PlotShips()

        private Coordinate DetermineStartPoint()
        {
            //Initialize
            CoordinateGenerator coordinateGenerator = new CoordinateGenerator(XSize, YSize);
            Coordinate newStartPoint = default;
            bool validStartPointFound = false, coordinateFound = false;
            int shipsWithNotCurrentCoordinate = 0, coordCount = 0;

            //Create a valid unused coordinate
            try
            {
                while (!validStartPointFound)
                {
                    //Create new Coordinates
                    newStartPoint = coordinateGenerator.GenerateNewCoordinate();

                    //If there is no active ships
                    if (activeShips.Count == 0)
                    {
                        validStartPointFound = true;
                    }
                    foreach (KeyValuePair<string, BattleShip> activeShip in ActiveShips)
                    {
                        coordCount = 0;
                        foreach (Coordinate activeCoordinates in activeShip.Value.OccupiedCoordinates)
                        {
                            //Checks if a coordinate is occupied
                            if ((activeCoordinates.X == newStartPoint.X) && (activeCoordinates.Y == newStartPoint.Y))
                            {
                                //If it is occupied
                                //Update Coordinate Generator
                                coordinateGenerator.availableX.Remove(newStartPoint.X);
                                coordinateGenerator.availableY.Remove(newStartPoint.Y);
                                coordinateGenerator.XLast = coordinateGenerator.XLast--;
                                coordinateGenerator.YLast = coordinateGenerator.YLast--;

                                //Exit both loops
                                coordinateFound = true;
                                break;
                            }
                            else
                            {
                                coordCount++;
                                if (coordCount == activeShip.Value.OccupiedCoordinates.Count)
                                {
                                    shipsWithNotCurrentCoordinate++;
                                }
                            }

                        }//foreach occupied coordinate

                        //Exit the outerloop
                        if (coordinateFound) { break; }

                    }//foreach ship

                    //Check if the coordinate is valid
                    if (shipsWithNotCurrentCoordinate == activeShips.Count)
                    {
                        validStartPointFound = true;
                    }

                }//while !validStartPoint

            }//try
            catch (Exception e)
            {
                MapEncounterdError("An error occurred while findign a startpoint", Convert.ToString(e));
            }


            return newStartPoint;

        }

        private void PlotRestOfTheShip(BattleShip ShipWhoNeedsCoords, Coordinate startingCoordinates)
        {
            //Initialize
            List<Directions> openDirections = new List<Directions>();

            //Check if there is space open to plot points
            try
            {
                //Left
                Coordinate shipMaxLengthFromStart = new Coordinate(startingCoordinates.X - (ShipWhoNeedsCoords.Length - 1), startingCoordinates.Y);
                if (!DoesCoordinatePairExist(shipMaxLengthFromStart))
                {
                    openDirections.Add(Directions.LEFT);
                }

                //Right
                shipMaxLengthFromStart.X = startingCoordinates.X + (ShipWhoNeedsCoords.Length - 1);
                if (!DoesCoordinatePairExist(shipMaxLengthFromStart))
                {
                    openDirections.Add(Directions.RIGHT);
                }

                //Up
                shipMaxLengthFromStart.X = startingCoordinates.X;
                shipMaxLengthFromStart.Y = startingCoordinates.Y + (ShipWhoNeedsCoords.Length - 1);
                if (!DoesCoordinatePairExist(shipMaxLengthFromStart))
                {
                    openDirections.Add(Directions.UP);
                }

                //Down
                shipMaxLengthFromStart.Y = startingCoordinates.Y - (ShipWhoNeedsCoords.Length - 1);
                if (!DoesCoordinatePairExist(shipMaxLengthFromStart))
                {
                    openDirections.Add(Directions.DOWN);
                }

                //Choose at random a direction from the available list
                Random indexGene = new Random();
                int index = 0;
                index = indexGene.Next(0, openDirections.Count - 1);

                //Plot rest of the boat from the selected direction
                Directions finalDirection = openDirections[index];
                Coordinate shipNewCoords = default;
                ShipWhoNeedsCoords.OccupiedCoordinates.Add(startingCoordinates);
                if (finalDirection == Directions.LEFT)
                {
                    for (int i = startingCoordinates.X - 1; i > startingCoordinates.X - ShipWhoNeedsCoords.Length; i--)
                    {
                        shipNewCoords.X = i;
                        if (shipNewCoords.Y != startingCoordinates.Y)
                        {
                            shipNewCoords.Y = startingCoordinates.Y;
                        }

                        ShipWhoNeedsCoords.OccupiedCoordinates.Add(shipNewCoords);
                    }
                }

                if (finalDirection == Directions.RIGHT)
                {
                    for (int i = startingCoordinates.X + 1; i < startingCoordinates.X + ShipWhoNeedsCoords.Length; i++)
                    {
                        shipNewCoords.X = i;
                        if (shipNewCoords.Y != startingCoordinates.Y)
                        {
                            shipNewCoords.Y = startingCoordinates.Y;
                        }

                        ShipWhoNeedsCoords.OccupiedCoordinates.Add(shipNewCoords);
                    }
                }

                if (finalDirection == Directions.UP)
                {
                    for (int i = startingCoordinates.Y + 1; i < startingCoordinates.Y + ShipWhoNeedsCoords.Length; i++)
                    {
                        if (shipNewCoords.X != startingCoordinates.X)
                        {
                            shipNewCoords.X = startingCoordinates.X;
                        }
                        shipNewCoords.Y = i;
                        ShipWhoNeedsCoords.OccupiedCoordinates.Add(shipNewCoords);
                    }
                }

                if (finalDirection == Directions.DOWN)
                {
                    for (int i = startingCoordinates.Y - 1; i > startingCoordinates.Y - ShipWhoNeedsCoords.Length; i--)
                    {
                        if (shipNewCoords.X != startingCoordinates.X)
                        {
                            shipNewCoords.X = startingCoordinates.X;
                        }
                        shipNewCoords.Y = i;
                        ShipWhoNeedsCoords.OccupiedCoordinates.Add(shipNewCoords);
                    }
                }
            }//try
            catch (Exception e)
            {
                MapEncounterdError("An error occured while plotting rest of ship", Convert.ToString(e));
            }


        }

        public bool DoesCoordinatePairExist(Coordinate givenCoordinates)
        {
            bool coordinateFound = false;
            int coordCount = 0, shipsWithNoCurrentCoordinate = 0;

            //Check if falls within map boundries
            if ((givenCoordinates.X < 0) || (givenCoordinates.X > xSize) || (givenCoordinates.Y < 0) || (givenCoordinates.Y > ySize))
            {
                return true;
            }

            //Check if is the first boat being addedd
            if (IsActiveRegistryEmpty())
            {
                return false;
            }

            //Check if pair exists in active registry
            foreach (KeyValuePair<string, BattleShip> activeShip in ActiveShips)
            {
                foreach (Coordinate activeCoordinates in activeShip.Value.OccupiedCoordinates)
                {
                    //Checks if a coordinate is occupied
                    if ((activeCoordinates.X == givenCoordinates.X) && (activeCoordinates.Y == givenCoordinates.Y))
                    {

                        //Exit both loops
                        coordinateFound = true;
                        break;
                    }
                    else
                    {
                        coordCount++;
                        if (coordCount == activeShip.Value.OccupiedCoordinates.Count)
                        {
                            shipsWithNoCurrentCoordinate++;
                        }
                    }

                }//foreach occupied coordinate

                //Exit the outerloop
                if (coordinateFound) { break; }
            }

            if (shipsWithNoCurrentCoordinate != activeShips.Count)
            {
                coordinateFound = false;
            }

            return coordinateFound;
        }

        public BattleShip DoesShipHaveCoordinate(Coordinate givenCoordinates)
        {
            bool coordinateFound = false;
            BattleShip foundShip = default;
            int coordCount = 0, shipsWithNoCurrentCoordinate = 0;

            //Check if falls within map boundries
            if ((givenCoordinates.X < 0) || (givenCoordinates.X > xSize) || (givenCoordinates.Y < 0) || (givenCoordinates.Y > ySize))
            {
                return null;
            }

            //Check if pair exists in active registry
            foreach (KeyValuePair<string, BattleShip> activeShip in ActiveShips)
            {
                foreach (Coordinate activeCoordinates in activeShip.Value.OccupiedCoordinates)
                {
                    //Checks if a coordinate is occupied
                    if ((activeCoordinates.X == givenCoordinates.X) && (activeCoordinates.Y == givenCoordinates.Y))
                    {

                        //Exit both loops
                        coordinateFound = true;
                        foundShip = activeShip.Value;
                        break;
                    }

                }//foreach occupied coordinate

                //Exit the outerloop
                if (coordinateFound) { break; }
            }

            return foundShip;
        }

        private void MapEncounterdError(string msg, string strError = "")
        {
            if (String.IsNullOrEmpty(strError))
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine($"{msg}: {strError}");
            }

        }

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

        public enum Directions
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

    }
}

