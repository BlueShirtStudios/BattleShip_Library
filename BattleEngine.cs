using fileHandlerComponents;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using static BattleShipCollection.Map;
using BattleEventArgs;

namespace BattleShipCollection
{
    public class BattleEngine
    {
        //Properties
        private Map playerMap = default;
        private Map botMap = default;
        private string messageFromPlayerResult;
        private string messageFromBotResult;
        private GameModes gameMode;
        private BotModes botMode;
        private CoordinateGenerator coordGenerator = default;

        //Events
        public event EventHandler<BattleEventArgs.GameWonEventArgs>? GameWon;
        public event EventHandler<BattleEventArgs.ShipSunkArgs>? ShipSunk;
        public event EventHandler<BattleEventArgs.ShotResultEventArgs>? ShotAttempt;
        public event EventHandler<BattleEventArgs.GameLossEventArgs>? GameLoose;
        public event EventHandler? GameEnd;

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

        private GameModes GameMode
        {
            get { return this.gameMode; }
            set { this.gameMode = value; }
        }
            
        private BotModes BotMode
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
                this.playerMap = new Map(xSize, ySize, "Player");
                this.botMap = new Map(xSize, ySize, "Bot");

                //Create our coordinate generator
                this.coordGenerator = new CoordinateGenerator(xSize, ySize);
            }
            catch (Exception e)
            {
                //error handling
            }

        }
        public void AddShipToMap(string shipName, int width, int length)
        {
            //Add a ship of your design to game map
            try
            {
                //Add a ship to the Player's Map
                PlayerMap.AddShip(new BattleShip(shipName, width, length));

                //It will also add a ship of the same specifications to the Bot's map
                BotMap.AddShip(new BattleShip(shipName, width, length));
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
                    Fireshot(attemptedShot, BotMap);

                    //Checks the win condition
                    CheckWinCondition(BotMap);
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
                    Fireshot(attemptedShot, BotMap);

                    //Check if game is won
                    CheckWinCondition(BotMap, PlayerMap);

                    //The Bot fires back
                    LetBotFireBack();

                    //Check if game is won
                    CheckWinCondition(PlayerMap, BotMap);
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

        //this will maybe go
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

        //this will maybe go
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

        //this will maybe go
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

        private void Fireshot(Coordinate shotCoord, Map firedMap)
        {
            //Checks if a ship has that coordinates on the map that was shot
            BattleShip shipThatWasHit = firedMap.DoesShipHaveCoordinate(shotCoord);
            ShotOutcome targetedCoord = ShotOutcome.NONE;

            //Checks if the ship was hit or not
            if (shipThatWasHit != null)
            {
                //It was a hit
                targetedCoord = ShotOutcome.HIT;
                shipThatWasHit.TakeDamage(shotCoord);

                //Add successfull coord to hit history
                firedMap.HitShots.Add(shotCoord);

                //If the fired ship is sinking
                if (CheckIfShipSunk(shipThatWasHit, firedMap))
                {
                    firedMap.ActiveShips.Remove(shipThatWasHit.GenerateShipKey(), out shipThatWasHit);
                    targetedCoord = ShotOutcome.SUNK;
                }
            }
            //If the shot was not a hit
            else
            {
                //Not Hit
                firedMap.MissedShots.Add(shotCoord);
                targetedCoord = ShotOutcome.MISS;
            }

            //Activate Result of shot
            ShotAttempt?.Invoke(this, new BattleEventArgs.ShotResultEventArgs(
                targetedCoord.ToString(),
                shotCoord.X,
                shotCoord.Y
                ));

        }

        private bool CheckIfShipSunk(BattleShip firedShip, Map shipMap)
        {
            //Checks if the ship has sunk
            if (firedShip.Health == 0)
            {
                //Creates an event argument if the ship is sunk
                ShipSunk?.Invoke(this, new BattleEventArgs.ShipSunkArgs(
                    firedShip.Name, 
                    10, 
                    GetAmountOfShots(shipMap)
                    ));

                //Notifies internal code that ship has sunk
                return true;
            }

            return false;
        }

        private void CheckWinCondition(Map firedMap, Map shooterMap)
        {
            if (firedMap.ActiveShips.Count == 0)
            {
                //Trigger the win event for the winning player
                GameWon?.Invoke(this, new BattleEventArgs.GameWonEventArgs(
                        100,
                        GetAmountOfShots(shooterMap),
                        shooterMap.Owner
                        ));

                //Trigger the loose event for the loosing player
                GameLoose?.Invoke(this, new BattleEventArgs.GameLossEventArgs(
                    100,
                    GetAmountOfShots(firedMap),
                    firedMap.Owner
                    ));

                //Notify that Game has end
                GameEnd?.Invoke(this, EventArgs.Empty);
            }
            
        }

        private void CheckWinCondition(Map firedMap)
        {
            if (firedMap.ActiveShips.Count == 0)
            {
                //Trigger the win event for the winning player
                GameWon?.Invoke(this, new BattleEventArgs.GameWonEventArgs(
                        100,
                        GetAmountOfShots(firedMap),
                        firedMap.Owner
                        ));

            }

        }

        private int GetAmountOfShots(Map passedMap)
        {
            return passedMap.HitShots.Count + passedMap.MissedShots.Count;
        }

        private void LetBotFireBack()
        {

            if (BotMode == BotModes.EASY)
            {
                Coordinate botShot = coordGenerator.GenerateNewCoordinate();
                Fireshot(botShot, PlayerMap);
            }
        }

    }

    //this will maybe go
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

    public enum ShotOutcome
    {
        NONE,
        HIT,
        MISS,
        SUNK
    }
}