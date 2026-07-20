using fileHandlerComponents;
using System;
using System.Runtime.CompilerServices;
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
        private BotModes botMode;
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
            

        public BotModes BotMode
        {
            get { return this.botMode; }
            set { this.botMode = value; }
        }

        public CoordinateGenerator CoordGenerator
        {
            get { return this.coordGenerator; }
            set { this.coordGenerator = value; }
        }

        public void SelectGameMode(string strMode)
        {
            try
            {
                //Extract from input which is used to determine the mode
                int mode = Convert.ToInt32(strMode[0].ToString());

                //Assign the game mode
                switch (mode)
                {
                    case 1:
                        GameMode = GameModes.ONEWAY;
                        break;

                    case 2:
                        GameMode = GameModes.TWOWAY;
                        break;

                    default:
                        GameMode = GameModes.NOWAY;
                        break;
                }
            }
            catch
            {
                GameMode = GameModes.NOWAY;
            }
        }

        public void SelectGameMode(int mode)
        {
            try
            {
                //Assign the game mode
                switch (mode)
                {
                    case 1:
                        GameMode = GameModes.ONEWAY;
                        break;

                    case 2:
                        GameMode = GameModes.TWOWAY;
                        break;

                    default:
                        GameMode = GameModes.NOWAY;
                        break;
                }
            }
            catch
            {
                GameMode = GameModes.NOWAY;
            }
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
        public void AddShipToMap(string shipName, int width, int height)
        {
            //Add a ship of your design to game map
            try
            {
                //Add a ship to the Player's Map
                PlayerMap.AddShip(new BattleShip(shipName, width, height));

                //It will also add a ship of the same specifications to the Bot's map
                BotMap.AddShip(new BattleShip(shipName, width, height));
            }
            catch
            {
                //error handeling
            }
        }

        private bool CanWeInitializeGame()
        {
            if (GameMode != GameModes.NOWAY)
            {
                return true;
            }
            else { return false; }
        }

        public void InitializeGame()
        {
            //Initialize the game if only the player fires

            //Checks if a game mode was selected
            if (!CanWeInitializeGame())
            {
                //error handling / message
                System.Environment.Exit(0);
            }

            //Checks which game was activated 
            switch (GameMode)
            {
                case GameModes.ONEWAY:
                    //Player Ships
                    BotMap.PlotShips();
                    break;

                case GameModes.TWOWAY:
                    //Player Ships being plotted
                    PlayerMap.PlotShips();

                    //Bot ship being plotted
                    BotMap.PlotShips();
                    break;

                default:
                    //error handling here
                    break;
            }
            
        }

        public void AttemptShot(int x, int y)
        {
            //Outward Facing Method allowing the Player to make a shot at a ship. It updated the messages for each entity on a hit.

            //Formats to Coordinate Format
            Coordinate attemptedShot = new Coordinate(x, y);

            //Depending on the game mode, the certain shoor action will be used
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
                    MessageFromBotResult = LetBotFireBack();
                }
                catch (Exception e)
                {
                    //exception handling
                }

            }

        }

        public string GetBotCoordinateState(int x, int y)
        {
            //Outward Facing Method allowing access to the status of the Coordinate.
            try
            {
                CoordStates state = GetCoordState(new Coordinate(x, y), BotMap);
                return Convert.ToString(state);
            }
            catch
            {
                return "No Coordinate Found.";
            }

        }

        public string GetPlayerCoordinateState(int x, int y)
        {
            //Outward Facing Method allowing access to the status of the Coordinate.
            try
            {
                CoordStates state = GetCoordState(new Coordinate(x, y), PlayerMap);
                return Convert.ToString(state);
            }
            catch
            {
                return "No Coordinate Found.";
            }

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
            //Creates a list of all available game modes avialable and returns a List
            List<string> modes = new List<string>();
            try
            {
                //Builds list from our enum
                foreach (GameModes mode in Enum.GetValues(typeof(GameModes)))
                {
                    if (mode != null)
                    {
                        modes.Add(Convert.ToString(mode));
                    }

                }

                //Returns the list with the fomated enum values
                return modes;
            }
            catch
            {
                //Error handling
                return modes;
            }
        }

        private bool IsMiss(Coordinate givenCoord, Map passedMap)
        {
            //Goes through past shots to check it is added to missed shots list
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
            //Goes through past shots to check it is added to hit shots list
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
            //Returns the state of a coordinate

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

        private string Fireshot(Coordinate shotCoord, Map firedMap)
        {
            //Checks if a ship has that coordinates on the map that was shot
            BattleShip shipThatWasHit = firedMap.DoesShipHaveCoordinate(shotCoord);
            string msg = default;

            //Checks if the ship was hit or not
            if (shipThatWasHit != null)
            {
                //It was a hit
                msg = "Shot was a Hit!";
                shipThatWasHit.Health -= 20;
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
            if (CheckWinCondition(firedMap))
            {
                GameWon?.Invoke(this, EventArgs.Empty);
            }

            //Returns the message crafted based on shot outcome.
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

        private int GetAmountOfShots(Map passedMap)
        {
            return passedMap.HitShots.Count + passedMap.MissedShots.Count;
        }

        private string LetBotFireBack()
        {

            string msg = default;
            if (BotMode == BotModes.EASY)
            {
                Coordinate botShot = coordGenerator.GenerateNewCoordinate();
                msg = Fireshot(botShot, PlayerMap);
            }

            return msg;
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
        NOWAY,
        ONEWAY,
        TWOWAY
        
    }

    public enum BotModes
    {
        EASY
    }
}