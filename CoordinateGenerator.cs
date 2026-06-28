using System;

namespace BattleShipCollection
{
    public class CoordinateGenerator
    {
        public List<int> availableX = new List<int>();
        public List<int> availableY = new List<int>();
        private Random randomGene = new Random();
        private int xFirst;
        private int xLast;
        private int yFirst;
        private int yLast;
        private int xIndex;
        private int yIndex;

        public int XFirst
        {
            get { return this.xFirst; }
            set { this.xFirst = value; }
        }
        public int XLast
        {
            get { return this.xLast; }
            set { this.xLast = value; }
        }

        public int YFirst
        {
            get { return this.yFirst; }
            set { this.yFirst = value; }
        }

        public int YLast
        {
            get { return this.yLast; }
            set { this.yLast = value; }
        }

        public int XIndex
        {
            get { return this.xIndex; }
            set { this.xIndex = value; }
        }

        public int YIndex
        {
            get { return this.yIndex; }
            set { this.yIndex = value; }
        }

        public CoordinateGenerator(int xSize, int ySize)
        {
            //List of available values
            this.availableX = InitializeAllowedXCoords(xSize);
            this.availableY = InitializeAllowedYCoords(ySize);

            //Last and First Values of the list 
            this.xLast = availableX[availableX.Count - 1];
            this.xFirst = availableX[0];
            this.yLast = availableY[availableY.Count - 1];
            this.yFirst = availableY[0];

            //Index of an coordinate that is selected
            this.xIndex = 0;
            this.yIndex = 0;
        }

        private List<int> InitializeAllowedXCoords(int xSize)
        {
            List<int> xCoords = new List<int>();

            for (int i = 0; i < xSize; i++)
            {
                xCoords.Add(i + 1);
            }

            return xCoords;

        }

        private List<int> InitializeAllowedYCoords(int ySize)
        {
            List<int> yCoords = new List<int>();

            for (int i = 0; i < ySize; i++)
            {
                yCoords.Add(i + 1);
            }

            return yCoords;

        }

        public Coordinate GenerateNewCoordinate()
        {
            //Get item indexes
            XIndex = randomGene.Next(XFirst, XLast + 1);
            YIndex = randomGene.Next(YFirst, YLast + 1);

            //Assign new coords
            int x = availableX[XIndex - 1];
            int y = availableY[YIndex - 1];

            //Create new coords struct and return
            Coordinate newCoord = new Coordinate(x, y);
            return newCoord;
        }
    }
}

