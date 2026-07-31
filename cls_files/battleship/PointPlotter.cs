using BattleExceptions;

namespace BattleShipCollection
{
    public class PointPlotter
    {
        private CoordinateGenerator coordGene = default;
        public Coordinate startPoint = default;
        public List<Coordinate> followingCoords = default;

        public List<Coordinate> alreadyComputedCoords = default;

        private CoordinateGenerator CoordGene { get; }
        public Coordinate StartPoint { get; set; }
        public List<Coordinate> CoordinateSet { get; private set; }
        private List<Coordinate> AlreadyComputedCoords { get; }

        public PointPlotter(int xRange, int yRange)
        {
            this.coordGene = new(xRange, yRange);
            this.startPoint = new Coordinate(0, 0);
            this.followingCoords = new();
            this.alreadyComputedCoords = new();
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

        private Directions DetermineOpenDirection(Coordinate startPoint, int coordinateCount)
        {
            List<Directions> openDirection = GetOpenDirections(startPoint, coordinateCount);
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
            //Build new coordinate combined with differnence ot check if that coordinate is open
            Coordinate checker = GetCoordinateFromDirection(dir, coord, dis);

            //Does it exist?
            if (DoesThisPairExist(checker))
            {
                return true;
            }
            else { return false; }
        }

        private int UpdateXLeft(int x, int distance)
        {
            return x - (distance--);
        }

        private int UpdateYUp(int y, int distance)
        {
            return y + (distance--);
        }

        private int UpdateXRight(int x, int distance)
        {
            return x + (distance--);
        } 

        private int UpdateYDown(int y, int distance)
        {
            return y - (distance--);
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
            Coordinate EndCoordinate = GetCoordinateFromDirection(dir, startCoordinate, distance);

            //Build the set accroding to the direction
            if (dir == Directions.LEFT)
            {
                finalSet = CreateLeftSet(startCoordinate.X, EndCoordinate.X, startCoordinate.Y);
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
                finalSet = CreateDownSet(startCoordinate.Y, EndCoordinate.Y, startCoordinate.X);
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
            for (int i = min; i < max + 1; i++)
            {
                coord.X = i++;
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
            for (int i = min; i < max + 1; i++)
            {
                coord.Y = i++;
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
            for (int i = max; i > min + 1; i--)
            {
                coord.X = i--;
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
            for (int i = max; i > min + 1; i--)
            {
                coord.Y = i--;
                posNumbers.Add(coord);
            }

            return posNumbers;
        }

        public void ResetPlotter()
        {

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