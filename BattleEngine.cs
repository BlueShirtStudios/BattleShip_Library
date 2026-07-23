using fileHandlerComponents;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using System.Linq;

using static BattleShipCollection.Map;
using BattleEventArgs;
using BattlePlayers;

namespace BattleShipCollection
{
    public class BattleEngine
    {
        //Properties
        private GameModes gameMode;
        private BotModes botMode;
        private Map playerMap = default;
        private Map botMap = default;
        private CoordinateGenerator coordGenerator = default;
        private Dictionary<BasePlayer, Map> activePlayRegistry = new();
        private ProfileManagerComponents.ProfileManager profileManager = new();

        //Public Events
        public event EventHandler<BattleEventArgs.GameWonEventArgs>? GameWon;
        public event EventHandler<BattleEventArgs.ShipSunkArgs>? ShipSunk;
        public event EventHandler<BattleEventArgs.ShotResultEventArgs>? ShotAttempt;
        public event EventHandler<BattleEventArgs.GameLossEventArgs>? GameLoose;
        public event EventHandler? GameEnd;

        //Internal Events
        internal event EventHandler<BattleEventArgs.ShotResultEventArgs>? OnHit;

        private Map PlayerMap
        {
            get { return this.playerMap; }
        }

        private Map BotMap
        {
            get { return this.botMap; }
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

        public Dictionary<BasePlayer, Map> ActivePlayRegistry
        {
            //Dictionary is structured as --> ActiveShoot: TargetMap
            get { return this.activePlayRegistry; }
        }

        public ProfileManagerComponents.ProfileManager ProfileManager
        {
            get { return this.profileManager; }
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
                //Build Active Registery
                BuildActiveRegistry(GameMode, xSize, ySize);

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
                foreach (KeyValuePair<BasePlayer, Map> register in ActivePlayRegistry)
                {
                    //Checks if a key has a map object, else it will not add the created boat
                    if (register.Value != null)
                    {
                        register.Value.AddShip(new BattleShip(shipName, width, length));
                    }
                }
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
            //Checks if a game mode was selected
            if (!CanWeInitializeGame())
            {
                //error handling / message
                System.Environment.Exit(0);
            }

            //Plots the ships on the map
            PlotShipsOnAllMaps();
        }

        private void BuildActiveRegistry(GameModes mode, int x, int y)
        {
            //Checks which game was activated 
            if (mode == GameModes.ONEWAY)
            {
                ActivePlayRegistry.Add(CreatePlayerObject(ProfileManager.CreateUserProfile()), new Map(x, y));
            }
            else if (mode == GameModes.TWOWAY)
            {
                ActivePlayRegistry.Add(CreatePlayerObject(ProfileManager.CreateUserProfile()), new Map(x, y));
                ActivePlayRegistry.Add(CreateBotPlayerObject(ProfileManager.CreateBotProfile(), BotMode), new Map(x, y));
            }
                
        }

        private BasePlayer CreatePlayerObject(UserProfile profile)
        {
            return new Player(profile);
        }

        private BasePlayer CreateBotPlayerObject(UserProfile profile, BotModes diff)
        {
            return new BotPlayer(diff, profile);
        }

        private void PlotShipsOnAllMaps()
        {
            foreach(KeyValuePair<BasePlayer, Map> register in ActivePlayRegistry)
            {
                //Checks if a key has a map object, else it will not plot
                if (register.Value != null)
                {
                    register.Value.PlotShips();
                }
            }
            
        }

        public void AttemptShot(int x, int y)
        {
            try
            {
                StartPlayerTurn();
                NewAttemptShot(x, y);
            }
            catch
            {
                //error handling
            }
        }

        private void StartPlayerTurn()
        {
            Player player = ActivePlayRegistry.Keys
                    .OfType<Player>()
                    .FirstOrDefault();

            player.GameData.ActivateTurn();

        }

        private void NewAttemptShot(int x, int y)
        {
            //Get Active Player and its target map
            BasePlayer activePlayer = GetActivePlayer();
            Map targetMap = activePlayer != null ? GetTargetMap(activePlayer) : null ;

            //Check if player is a bot and resolve coordinate allowcation if so
            Coordinate targetedCoordinate = ResolveBotCoords(activePlayer);

            //Create coordinate if active player is a actual person
            if ((targetedCoordinate.X == 0) || (targetedCoordinate.Y == 0))
            {
                targetedCoordinate = new Coordinate(x, y);
            }

            //Fires a shot on the active map
            ShotOutcome outcome = Fireshot(targetedCoordinate, targetMap);

            //Updates bot's result if player is bot
            UpdateBotShotResult(activePlayer, targetedCoordinate, outcome);

            //Handle post shot logic
            HandlePostShot(activePlayer, targetMap);
        }

        private BasePlayer GetActivePlayer()
        {
            return ActivePlayRegistry.Keys.FirstOrDefault(p =>
                p != null &&
                p.GameData != null &&
                p.GameData.IsTurn
                );
        }

        private Map GetTargetMap(BasePlayer activePlayer)
        {
            var targetMap = ActivePlayRegistry[activePlayer];
            return targetMap != null ? targetMap : null;
        }

        private Coordinate ResolveBotCoords(BasePlayer player)
        {
           if (player is BotPlayer bot)
           {
                return bot.CalculateNextCoordinate();
           }

            return new Coordinate(0,0);
        }

        private void UpdateBotShotResult(BasePlayer player, Coordinate coord, ShotOutcome outcome)
        {
            if (player is BotPlayer bot)
            {
                bot.UpdateShotOutcome(outcome, coord);
            }
        }

        private BasePlayer GetOppenentPlayer()
        {
            return ActivePlayRegistry.Keys.FirstOrDefault(p =>
                p != null &&
                p.GameData != null &&
                !p.GameData.IsTurn
                );
        }

        private void HandlePostShot(BasePlayer activePlayer, Map targetMap)
        {
            int PLAYERS = ActivePlayRegistry.Count;

            //Updates game score
            UpdateGameStats(activePlayer, 100);

            //Checks win condition and how to raise win event
            if (CheckWinCondition(targetMap))
            {
                if (PLAYERS == 1)
                {
                    RaiseWinEvent(activePlayer);
                }
                else if (PLAYERS == 2)
                {
                    RaiseWinEvent(activePlayer, GetOppenentPlayer());
                }

            }//if
            else { PrepareForNextTurn(activePlayer, GetOppenentPlayer()); }

        }

        private void PrepareForNextTurn(BasePlayer currentAcivePlayer, BasePlayer currentInactivePlayer)
        {
            currentAcivePlayer.GameData.EndTurn();
            currentInactivePlayer.GameData.ActivateTurn();

            if (currentInactivePlayer is BotPlayer)
            {
                NewAttemptShot(0, 0);
            }
        }

        private int GetNextOrPreviousIndex(int num)
        {
            //Based on the integer provided it will return the previous number or next one
            if (num == 0)
            {
                num++;
            }
            else
            {
                num--;
            }

            return num;
        }
        private BasePlayer FetchPlayerFromRegister(int index)
        {
            var allKeys = ActivePlayRegistry.Keys.ToList();
            if (index < allKeys.Count)
            {
                return allKeys[index];
            }
            else { return null; }
        }
        private void UpdateGameStats(BasePlayer activeShooter, int score)
        {
            activeShooter.GameData.UpdateScore(score);
            activeShooter.GameData.UpdateShotsMade();
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

        private ShotOutcome Fireshot(Coordinate shotCoord, Map firedMap)
        {
            //Checks if a ship has that coordinates on the map that was shot
            BattleShip shipThatWasHit = firedMap.DoesShipHaveCoordinate(shotCoord);
            ShotOutcome targetedCoordOutcome = ShotOutcome.NONE;

            //Checks if the ship was hit or not
            if (shipThatWasHit != null)
            {
                //It was a hit
                targetedCoordOutcome = ShotOutcome.HIT;
                shipThatWasHit.TakeDamage(shotCoord);

                //Add successfull coord to hit history
                firedMap.HitShots.Add(shotCoord);

                //If the fired ship is sinking
                if (CheckIfShipSunk(shipThatWasHit, firedMap))
                {
                    firedMap.ActiveShips.Remove(shipThatWasHit.GenerateShipKey(), out shipThatWasHit);
                    targetedCoordOutcome = ShotOutcome.SUNK;
                }
            }
            //If the shot was not a hit
            else
            {
                //Not Hit
                firedMap.MissedShots.Add(shotCoord);
                targetedCoordOutcome = ShotOutcome.MISS;
            }

            //Trigger Shot Result Event
            RaiseShotResultEvent(targetedCoordOutcome, shotCoord.X, shotCoord.Y);

            //Return result of the shot
            return targetedCoordOutcome;
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

        private bool CheckWinCondition(Map firedMap)
        {
            if (firedMap.ActiveShips.Count == 0)
            {
                return true;

            }
            else { return false; }

        }

        private void RaiseWinEvent(BasePlayer winner)
        {
            GameWon?.Invoke(this, new BattleEventArgs.GameWonEventArgs(
                        winner.GameData.Score,
                        winner.GameData.ShotsMade,
                        winner.Profile.displayName
                        ));

            RaiseGameEnd();
        }

        private void RaiseWinEvent(BasePlayer winner, BasePlayer loser)
        {
            //Trigger the win event
            GameWon?.Invoke(this, new BattleEventArgs.GameWonEventArgs(
                        winner.GameData.Score,
                        winner.GameData.ShotsMade,
                        winner.Profile.displayName
                        ));

            //Trigger the loose event
            GameLoose?.Invoke(this, new BattleEventArgs.GameLossEventArgs(
                loser.GameData.Score,
                loser.GameData.ShotsMade,
                loser.Profile.displayName
                ));

            RaiseGameEnd();
        }

        private void RaiseGameEnd()
        {
            GameEnd?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOnHitEvent(ShotOutcome outcome, Coordinate firedCoords)
        {
            OnHit?.Invoke(this, new BattleEventArgs.ShotResultEventArgs(
                outcome,
                firedCoords.X,
                firedCoords.Y
                ));
        }

        private void RaiseShotResultEvent(ShotOutcome outcome, int x, int y)
        {
            ShotAttempt?.Invoke(this, new BattleEventArgs.ShotResultEventArgs(
                outcome,
                x,
                y
                ));
        }

        private int GetAmountOfShots(Map passedMap)
        {
            return passedMap.HitShots.Count + passedMap.MissedShots.Count;
        }
    

    }//battle engine class

    public enum GameModes
    {
        NOWAY,
        ONEWAY,
        TWOWAY
        
    }

    public enum ShotOutcome
    {
        NONE,
        HIT,
        MISS,
        SUNK
    }
}