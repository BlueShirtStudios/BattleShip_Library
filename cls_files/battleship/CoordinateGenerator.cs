using System;

namespace BattleShipCollection
{
    public class CoordinateGenerator
    {
        private  List<int> availableX = new List<int>();
        private List<int> availableY = new List<int>();
        private Random randomGene = new Random();
        public int XFirst { get; set; }
        public int XLast { get; set; }
        public int YFirst { get; set; }
        public int YLast { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public int XBoundryMax { get; set; }
        public int YBoundryMax { get; set; }

        private List<int> AvailableX
        {
            get { return this.availableX; }
            set { this.availableX = value; }
        }
        private List<int> AvailableY
        {
            get { return this.availableY; }
            set { this.availableY = value; }
        }
        private Random RandomGene
        {
            get { return this.randomGene; }
        }

        public CoordinateGenerator(int xBoundry, int yBoundry)
        {
            //Set Max Boundries
            XBoundryMax = xBoundry;
            YBoundryMax = yBoundry;

            AvailableX = CreateListOfApprovedValues(XBoundryMax, AvailableX);
            AvailableY = CreateListOfApprovedValues(YBoundryMax, AvailableY);

            //Set Tracker Variables
            SetMaxAndMin();

            //Set Index trackers
            ResetIndexTrackers();
        }

        private List<int> CreateListOfApprovedValues(int maxBoundry, List<int> lst)
        {
            //Takes a list with its boundry to create a list from 1 -> max boundry to select numbers from
            for (int i = 0; i < maxBoundry; i++)
            {
                //Adds number to the list
                lst.Add(i + 1);
            }

            //Return the generated lisr
            return lst;
        }

        private void SetMaxAndMin()
        {
            XFirst = 0;
            XLast = XBoundryMax;
            YLast = 0;
            YLast = YBoundryMax;
        }

        private void ResetIndexTrackers()
        {
            XIndex = 0;
            YIndex = 0;
        }

        private int GenerateNumber(int first, int last, List<int> nums)
        {
            //Generate a random index from the ranges
            int index = RandomGene.Next(first, last++);

            //Fetch number from corresponding index
            int num = nums[index--];

            //Return the number
            return num;
        }

        private bool IsInBoundry(int target, int boundry)
        {
            //Checks if target numeber is within boundry
            if ((target > 0) && (target <= boundry))
            {
                //If the target is within the boundry
                return true;
            }
            //If the target is not within the boundry
            else { return false; }
        }

        public Coordinate GenerateNewCoordinate()
        {
            //Generates a new coordinate and returns it
            //Initialize
            int x = 0, y = 0;

            //Generate x value
            while (!IsInBoundry(x, XBoundryMax))
            {
                x = GenerateNumber(XFirst, YLast, AvailableX);
            }

            //Generate y value
            while(!IsInBoundry(y, YBoundryMax))
            {
                y = GenerateNumber(YFirst, YLast, AvailableY);
            }

            return new Coordinate(x, y);
        }
    }
}

