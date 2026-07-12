using fileHandlerComponents;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using static BattleShipCollection.Map;

namespace BattleShipCollection
{
    public class BattleEngine
    {
        private Map playerMap = default;
        private Map botMap = default;
        private string messageFromPlayerResult;
        private string messageFromBotResult;
        private GameModes gameMode;
        private CoordinateGenerator coordGenerator = default;

        public event EventHandler GameWon;

        private Map PlayerMap
        {
            get { return this.playerMap; }
        }

        private Map BotMap
        {
            get { return this.botMap; }
        }

        public string MessageFromPlayerResult
        {
            get { return this.messageFromPlayerResult; }
            set { this.messageFromPlayerResult = value; }
        }


        public string MessageFromBotResult
        {
            get { return this.messageFromBotResult; }
            set { this.messageFromBotResult = value; }
        }

        public GameModes GameMode
        {
            get { return this.gameMode; }
            set { this.gameMode = value; }
        }

        public CoordinateGenerator CoordGenerator
        {
            get { return this.coordGenerator; }
            set { this.coordGenerator = value; }
        }

        public void SelectGameMode(string gameMode)
        {

        }

        public void CreateGameMap(int xSize, int ySize)
        {
            try
            {
                //For both entities, the same map size will be created
                this.playerMap = new Map(xSize, ySize);
                this.botMap = new Map(xSize, ySize);

                //Create our coordinate generator
                this.coordGenerator = new CoordinateGenerator(xSize, ySize);
            }
            catch (Exception e)
            {
                //error handling
            }

        }
        public void AddShipToPlayerMap(string shipName, int width, int height)
        {
            try
            {
                PlayerMap.AddShip(new BattleShip(shipName, width, height));
            }
            catch
            {
                //error handeling
            }
        }

        public void Initialize1WayGame()
        {
            //Player Ships
            BotMap.PlotShips();

            //Set events
           // BotMap.GameWon += this.HandleVictory;
        }

        public void Initialize2WayGAame()
        {
            //Player Ships being plotted
            PlayerMap.PlotShips();

            //Bot ship being plotted
            BotMap.PlotShips();

        }

        public void AttemptShot(int x, int y)
        {
            Coordinate attemptedShot = new Coordinate(x, y);

            if (GameMode == GameModes.ONEWAY)
            {
                try
                {
                    //The client gets to fire
                    MessageFromPlayerResult = Fireshot(attemptedShot, BotMap);
                }
                catch (Exception e)
                {
                    //exception handling
                }
            }

            if (GameMode == GameModes.TWOWAY)
            {
                try
                {
                    //The client gets to fire
                    MessageFromPlayerResult = Fireshot(attemptedShot, BotMap);

                    //The Bot fires back
                }
                catch (Exception e)
                {
                    //exception handling
                }

            }

        }

        public string GetBotCoordinateState(int x, int y)
        {
            CoordStates state = BotMap.GetCoordState(x, y);
            return Convert.ToString(state);

        }

        public void HandlePlayerVictory(object sender, EventArgs e)
        {
            MessageFromPlayerResult = $"You have won!!!";
        }

        public void HandleBotVictory(object sender, EventArgs e)
        {
            MessageFromBotResult = $"The Bot has won have won!!!";
        }

        public List<string> GetAllAvailableModes()
        {
            List<string> modes = new List<string>();
            foreach (GameModes mode in Enum.GetValues(typeof(GameModes)))
            {
                
                modes.Add(Convert.ToString(mode));

            }

            return modes;

        }

        private bool IsMiss(Coordinate givenCoord, Map passedMap)
        {
            foreach (Coordinate coord in passedMap.MissedShots)
            {
                if ((coord.X == givenCoord.X) && (coord.Y == givenCoord.Y))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsHit(Coordinate givenCoord, Map passedMap)
        {
            foreach (Coordinate coord in passedMap.HitShots)
            {
                if ((coord.X == givenCoord.X) && (coord.Y == givenCoord.Y))
                {
                    return true;
                }
            }

            return false;
        }

        private CoordStates GetCoordState(Coordinate givenCoord, Map passedMap)
        {
            CoordStates coordState = default;
            //Check if occupied by ship
            if (IsMiss(givenCoord, passedMap))
            {
                coordState = CoordStates.MISS;
            }
            else if (IsHit(givenCoord, passedMap))
            {
                coordState = CoordStates.HIT;
            }
            else { coordState = CoordStates.WATER; }

            return coordState;

        }

        public CoordStates GetCoordState(int x, int y)
        {
            Coordinate givenCoord = new Coordinate(x, y);
            CoordStates coordState = default;
            //Check if occupied by ship
            if (IsMiss(givenCoord))
            {
                coordState = CoordStates.MISS;
            }
            else if (IsHit(givenCoord))
            {
                coordState = CoordStates.HIT;
            }
            else { coordState = CoordStates.WATER; }

            return coordState;

        }

        private string Fireshot(Coordinate shotCoord, Map firedMap)
        {
            //Checks if a ship has that coordinates on the bot map
            BattleShip shipThatWasHit = firedMap.DoesShipHaveCoordinate(shotCoord);
            string msg = default;

            //Checks if the ship was hit or not
            if (shipThatWasHit != null)
            {
                //It was a hit
                msg = "Shot was a Hit!";
                shipThatWasHit.Health = shipThatWasHit.Health - 20;
                shipThatWasHit.OccupiedCoordinates.Remove(shotCoord);
                if (shipThatWasHit.Health == 0)
                {
                    firedMap.ActiveShips.Remove(shipThatWasHit.GenerateShipKey(), out shipThatWasHit);
                    msg = SunkMessage(shipThatWasHit);
                }
                firedMap.HitShots.Add(shotCoord);

            }
            else if (shipThatWasHit == null)
            {
                //Not Hit
                firedMap.MissedShots.Add(shotCoord);
                msg = "Shot was a Miss.";
            }

            //Check if game is won
            if (CheckWinCondition())
            {
                GameWon?.Invoke(this, EventArgs.Empty);
            }

            return msg;
        }

        private string SunkMessage(BattleShip sunkenShip)
        {
            return $"You have sunk the enemy's {sunkenShip.Name}, well done!";
        }


        private bool CheckWinCondition(Map passedMap)
        {
            if (passedMap.ActiveShips.Count == 0)
            {
                return true;
            }
            else { return false; }
        }

        public int GetAmountOfShots(Map passedMap)
        {
            return passedMap.HitShots.Count + passedMap.MissedShots.Count;
        }
    }

    public enum CoordStates
    {
        HIT,
        MISS,
        WATER,
        SHIP
    }

    public enum GameModes
        {
            ONEWAY,
            TWOWAY
        }
    }
}

