namespace BattleShipCollection
{
    public class PointPlotter
    {
        private CoordinateGenerator coordGene = default;
        public Coordinate startPoint = default;
        public List<Coordinate> followingCoords = default;

        public Dictionary<Coordinate, string> alreadyComputedCoords = default;

        private CoordinateGenerator CoordGene { get; }
        public Coordinate StartPoint { get; set; }
        public List<Coordinate> CoordinateSet { get; }
        private Dictionary<Coordinate, string> AlreadyComptedCoords { get; }


        public PointPlotter(int xRange, int yRange)
        {
            this.coordGene = new(xRange, yRange);
            this.startPoint = new Coordinate(0, 0);
            this.followingCoords = new();
            this.alreadyComputedCoords = new();
        }

        public void CreateShipCoordinateSet()
        {

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
            }

        }

        
        private Coordinate GenerateCoordinate()
        {

            return CoordGene.GenerateNewCoordinate();
        }

        private bool DoesThisPairExist(Coordinate coord)
        {

        }

    }
}