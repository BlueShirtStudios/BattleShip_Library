using BattleShipCollection;

namespace BattlePlayers
{
    public class BotMoves
    {
        public Coordinate PreviousCoordiate { get; set; }
        public ShotOutcome CoordinateOutcome { get; set; }
        public CoordinateGenerator CoordinateGene { get; set; }

        public List<Coordinate> hitShots = new();

        public List<Coordinate> missedShots = new();

        public List<Coordinate> assumedCurrentTarget = new();

        public List<Coordinate> HitShots
        {
            get { return this.hitShots; }
            private set { this.hitShots = value; }
        }

        public List<Coordinate> MissedShots
        {
            get { return this.missedShots; }
            private set { this.missedShots = value; }
        }

        public List<Coordinate> AssumedCurrentTarget
        {
            get { return this.assumedCurrentTarget; }
        }

        public BotMoves(CoordinateGenerator coordinateGenerator)
        {
            this.CoordinateGene = coordinateGenerator;
        }

        public void UpdateCoordinateRegister(List<Coordinate> hits, List<Coordinate> misses)
        {
            MissedShots = misses;
            HitShots = hits;
        }

        public Coordinate DetermineMoveEasy()
        {
            return CoordinateGene.GenerateNewCoordinate();
        }

        public Coordinate DetermineMoveMedium()
        {
            Coordinate chosenCoordinate = default;

            //If MISS OR NONE
            if ((CoordinateOutcome == ShotOutcome.NONE) || (CoordinateOutcome == ShotOutcome.MISS))
            {
                chosenCoordinate = GenerateGeneralCoordinate();
            }
            //If HIT
            else if (CoordinateOutcome == ShotOutcome.HIT)
            {
                //Add successfull hit to our assumed target coordinate list
                AssumedCurrentTarget.Add(PreviousCoordiate);
                chosenCoordinate = CoordinateIfHit();
            }
            //If SUNK
            else if(CoordinateOutcome == ShotOutcome.SUNK)
            {
                //Clears because target is sunk
                AssumedCurrentTarget.Clear();

                //Creates new coord
                chosenCoordinate = GenerateGeneralCoordinate();
            }

            return chosenCoordinate;

        }

        private Coordinate CoordinateIfMiss()
        {
            return GenerateGeneralCoordinate();
        }

        private Coordinate GenerateGeneralCoordinate()
        {
            //Initialize
            bool approved = false;
            Coordinate geneCoord = new(0, 0);

            //Loops until approved coordinate is generated
            while (!approved)
            {
                geneCoord = CoordinateGene.GenerateNewCoordinate();
                if (!DoesThisPairExists(geneCoord))
                {
                    approved = true;
                }
            }

            //When coordinate is approved it will return the approved coordinate
            return geneCoord;
        }

        private bool DoesThisPairExists(Coordinate coord)
        {
            bool valid = false;
            //Checks missed shot list
            foreach(Coordinate c in MissedShots)
            {
                if ((c.X == coord.X) && (c.Y == coord.Y))
                {
                    valid = true;
                }
                else { valid = false; }
            }

            //Checks hit shot list
            foreach (Coordinate c in HitShots)
            {
                if ((c.X == coord.X) && (c.Y == coord.Y))
                {
                    valid = true;
                }
                else { valid = false; }
            }

            //Return final outcome
            return valid;
        }

        private bool DoesThisPairExists(Coordinate coord, List<Coordinate> lst)
        {
            bool valid = false;

            foreach(Coordinate c in lst)
            {
                if ((c.X == coord.X) && (c.Y == coord.Y))
                {
                    valid = true;
                }
                else { valid = false; }
            }

            return valid;
        }

        private Coordinate CoordinateIfHit()
        {
            //Checks how many coords were hits
            int coordsThatWereHits = AssumedCurrentTarget.Count;
            Coordinate newCoord = default;

            //If only one coordinate is in our assumed target
            if (coordsThatWereHits == 1)
            {
                //Creates seperate coord generator to fit the assumed hits from the first hit coord
                CoordinateGenerator currentGene = new(PreviousCoordiate.X + 1, PreviousCoordiate.Y + 1);

                //Continue to generate coords until one does not exist in the current assumed target list
                do
                {
                    newCoord = currentGene.GenerateNewCoordinate();

                } while (DoesThisPairExists(newCoord, AssumedCurrentTarget));
                 
            }

            //If there are 2 or more hits in the assuemed target list
            else if (coordsThatWereHits >= 2)
            {
                //Checks what value stayed the same
                //If X value stayed the same
                if ((AssumedCurrentTarget[0].X == PreviousCoordiate.X) && (AssumedCurrentTarget[1].X == PreviousCoordiate.X))
                {
                    //Creates Y coordinate +1 current successfull hit
                    newCoord = new Coordinate(PreviousCoordiate.X, PreviousCoordiate.Y + 1);
                }
                //Else creates X value + 1 successfull hit
                else { newCoord = new Coordinate(PreviousCoordiate.X + 1, PreviousCoordiate.Y); }
                
            }

            //Return the new Coordinate 
            return newCoord;
        }


    }
}