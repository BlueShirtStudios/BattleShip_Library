using BattleExceptions;

namespace BattleShipCollection
{
    public class PointPlotter
    {
        private CoordinateGenerator coordGene = null;

    //    public List<Coordinate> alreadyComputedCoords = new();

        private CoordinateGenerator CoordGene
        {
            get { return this.coordGene; }
        }
        public Coordinate StartPoint { get; set; }
        public List<Coordinate> CoordinateSet { get; private set; }
        private List<Coordinate> AlreadyComputedCoords { get; }

        public PointPlotter(int xRange, int yRange)
        {
            this.coordGene = new(xRange, yRange);
            this.StartPoint = new Coordinate(0, 0);
            this.CoordinateSet = new();
            this.AlreadyComputedCoords = new();
        }

        public void CreateShipCoordinateSet(int shipSize)
        {
            try
            {
                StartPoint = GetStartingPoint();
                Directions chosenDirection = DetermineOpenDirection(StartPoint, shipSize);
                CoordinateSet = CreateCoordinateSet(chosenDirection, StartPoint, shipSize);
            }
            catch (CoordinateOutOfRangeError e)
            {
                
            }
            
        }

        private Coordinate GetStartingPoint()
        {
            //Gets the starting coordinate of a ship
            //Initialize
            Coordinate startPoint = default;
            bool foundValidStartPoint = false;

            //Start searching for the valid start point
            while (!foundValidStartPoint)
            {
                startPoint = GenerateCoordinate();
                if (!DoesThisPairExist(startPoint))
                {
                    foundValidStartPoint = true;
                }
            }

            //When the startpoint is approved
            return startPoint;
        }

        
        private Coordinate GenerateCoordinate()
        {

            return CoordGene.GenerateNewCoordinate();
        }

        private bool DoesThisPairExist(Coordinate coord)
        {
            //Checks if this pair was computed
           foreach(Coordinate c in AlreadyComputedCoords)
           {
                if ((coord.X == c.X) && (coord.Y == c.Y))
                {
                    return true;
                }
           }

            return false;
        }

        private Directions DetermineOpenDirection(Coordinate startPoint, int shipSize)
        {
            List<Directions> openDirection = GetOpenDirections(startPoint, shipSize - 1); //shipSize - 1 = Distance from already found startpoint to supposed end
            return ChooseDirection(openDirection);
        }

        private List<Directions> GetOpenDirections(Coordinate coord, int distanceFromCoord)
        {
            List<Directions> open = new();

            //Loops through all available direction to check if is open
            foreach(Directions direction in Enum.GetValues<Directions>())
            {
                if (IsThisDirectionOpen(direction, coord, distanceFromCoord))
                {
                    open.Add(direction);
                }
            }

            return open;
       
        }

        private Coordinate GetCoordinateFromDirection(Directions dir, Coordinate coord, int dis)
        {
            //Update tthe coordinate based on the direction that is passed
            switch (dir)
            {
                case Directions.LEFT:
                    {
                        coord.X = UpdateXLeft(coord.X, dis);
                        break;
                    }

                case Directions.RIGHT:
                    {
                        coord.X = UpdateXRight(coord.X, dis);
                        break;
                    }

                case Directions.UP:
                    {
                        coord.Y = UpdateYUp(coord.Y, dis);
                        break;
                    }

                case Directions.DOWN:
                    {
                        coord.Y = UpdateYDown(coord.Y, dis);
                        break;
                    }
            }

            //Return the coordinate after it changed
            return coord;
        }

        private bool IsThisDirectionOpen(Directions dir, Coordinate coord, int dis)
        {
            //Build new coordinate combined with differnence and check if that coordinate is open
            Coordinate checker = GetCoordinateFromDirection(dir, coord, dis);

            //Is the coordinate within boundries
            if (!IsWithinBoundries(checker))
            {
                return false;
            }

            //Does it already exist within computed coordinates?
            if (DoesThisPairExist(checker))
            {
                //If the coordinate exists, the direction is taken
                return false;
            }

            //If is does not exist, the direction is open
            else { return true; }
        }

        private bool IsWithinBoundries(Coordinate coord)
        {
            bool checker = false;
            //Checks if the coordinate values are not 0
            if ((coord.X > 0) && (coord.Y > 0))
            {
                //Check is the value of the coordinate falls within the range
                if ((coord.X <= CoordGene.XBoundryMax) && (coord.Y <= CoordGene.YBoundryMax))
                {
                    checker = true;
                }
                
            }
            return checker;
        }

        private int UpdateXLeft(int x, int distance)
        {
            return x - (distance);
        }

        private int UpdateYUp(int y, int distance)
        {
            return y + (distance);
        }

        private int UpdateXRight(int x, int distance)
        {
            return x + (distance);
        } 

        private int UpdateYDown(int y, int distance)
        {
            return y - (distance);
        }

        private Directions ChooseDirection(List<Directions> availableDir)
        {
            Random gene = new Random();
            int index = gene.Next(0, availableDir.Count);
            return availableDir[index];
        }

        private List<Coordinate> CreateCoordinateSet(Directions dir, Coordinate startCoordinate, int distance)
        {
            //Initialize
            List<Coordinate> finalSet = new();

            //Get the end coordinate
            Coordinate EndCoordinate = GetCoordinateFromDirection(dir, startCoordinate, distance - 1);

            //Build the set accroding to the direction
            if (dir == Directions.LEFT)
            {
                finalSet = CreateLeftSet(EndCoordinate.X, startCoordinate.X, startCoordinate.Y);
            }
            else if (dir == Directions.RIGHT)
            {
                finalSet = CreateRightSet(startCoordinate.X, EndCoordinate.X, startCoordinate.Y);
            }
            else if (dir == Directions.UP)
            {
                finalSet = CreateUpSet(startCoordinate.Y, EndCoordinate.Y, startCoordinate.X);
            }
            else if (dir == Directions.DOWN)
            {
                finalSet = CreateDownSet(EndCoordinate.Y, startCoordinate.Y, startCoordinate.X);
            }

            //Return the new set
            return finalSet;
        }

        private List<Coordinate> CreateRightSet(int min, int max, int constant)
        {
            //Initialize
            List<Coordinate> posNumbers = new();
            Coordinate coord = new(0, constant);

            //Loop and build set
            for (int i = min; i <= max; i++)
            {
                coord.X = i + 1;
                posNumbers.Add(coord);
            }

            return posNumbers;
        }

        private List<Coordinate> CreateUpSet(int min, int max, int constant)
        {
            //Initialize
            List<Coordinate> posNumbers = new();
            Coordinate coord = new(constant, 0);

            //Loop and build set
            for (int i = min; i <= max; i++)
            {
                coord.Y = i + 1;
                posNumbers.Add(coord);
            }

            return posNumbers;
        }

        private List<Coordinate> CreateLeftSet(int min, int max, int constant)
        {
            //Initialize
            List<Coordinate> posNumbers = new();
            Coordinate coord = new(0, constant);

            //Loop and build set
            for (int i = max; i >= min; i--)
            {
                coord.X = i - 1;
                posNumbers.Add(coord);
            }

            return posNumbers;
        }

        private List<Coordinate> CreateDownSet(int min, int max, int constant)
        {
            //Initialize
            List<Coordinate> posNumbers = new();
            Coordinate coord = new(constant, 0);

            //Loop and build set
            for (int i = max; i >= min; i--)
            {
                coord.Y = i - 1;
                posNumbers.Add(coord);
            }

            return posNumbers;
        }

        public void ResetPlotter()
        {
            StartPoint = new Coordinate(0, 0);
            CoordinateSet.Clear();
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