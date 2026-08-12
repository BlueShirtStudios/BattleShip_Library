using fileHandlerComponents;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using System.Linq;

using static BattleShipCollection.Map;
using BattleEventArgs;
using BattlePlayers;
using BattleExceptions;
using BattleEnumCollection;
using System.Formats.Asn1;

namespace BattleShipCollection
{
    public class BattleEngine
    {
        //Properties
        private GameModes gameMode;
        private BotModes botMode;
        private Dictionary<BasePlayer, Map> activePlayRegistry = new();
        private ProfileManagerComponents.ProfileManager profileManager = new();

        //Public Events
        public event EventHandler<BattleEventArgs.GameWonEventArgs>? GameWon;
        public event EventHandler<BattleEventArgs.ShipSunkArgs>? ShipSunk;
        public event EventHandler<BattleEventArgs.ShotResultEventArgs>? ShotAttempt;
        public event EventHandler<BattleEventArgs.GameLossEventArgs>? GameLoose;
        public event EventHandler? GameEnd;
        public event EventHandler<BattleEventArgs.ErrorEventArgs>? ErrorOccurred;

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

        public void SelectBotDifficulty(int diff)
        {
            switch (diff)
            {
                case 1:
                    BotMode = BotModes.EASY;
                    break;

                case 2:
                    BotMode = BotModes.MEDIUM;
                    break;

                default:
                    BotMode = BotModes.NODIFF;
                    break;
            }
        }

        public void SelectBotDifficulty(string diff)
        {
            if (GameMode != GameModes.TWOWAY)
            {
                //raise error
            }

            //Get first letter of diff
            char cDiff = Char.ToLower(diff[0]);

            switch (cDiff)
            {
                case 'e':
                    BotMode = BotModes.EASY;
                    break;

                case 'm':
                    BotMode = BotModes.MEDIUM;
                    break;

                default:
                    BotMode = BotModes.NODIFF;
                    break;
            }
        }

        public void CreateGameMap(int xSize, int ySize)
        {
            try
            {
                //Build Active Registery
                BuildActiveRegistry(GameMode, xSize, ySize);

            }
            catch (Exception e)
            {
                //error handling
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

        public void InitializeGame(EngineConfig cfg)
        {
            try
            {
                //Extract configuration data from config object
                GameMode = cfg.GameMode;
                BotMode = cfg.Difficulty;
                BuildActiveRegistry(GameMode, cfg.xSize, cfg.ySize);

                //Plots the ships on the map
                PlotShipsOnAllMaps(cfg.RequesteShips);
            }
            catch
            {

            }
            
        }

        private void BuildActiveRegistry(GameModes mode, int x, int y)
        {
            //Build our active registry, handling map creation and coordinate generator
            //Checks which game was activated 
            if (mode == GameModes.ONEWAY)
            {
                ActivePlayRegistry.Add(CreatePlayerObject(ProfileManager.CreateUserProfile()), new Map(x, y));
            }
            else if (mode == GameModes.TWOWAY)
            {
                //Create Player object and bot object and add to dictionary
                ActivePlayRegistry.Add(CreatePlayerObject(ProfileManager.CreateUserProfile()), new Map(x, y));
                ActivePlayRegistry.Add(CreateBotPlayerObject(ProfileManager.CreateBotProfile(), BotMode, new CoordinateGenerator(x, y)), new Map(x, y));
            }
                
        }

        private BasePlayer CreatePlayerObject(UserProfile profile)
        {
            return new Player(profile);
        }

        private BasePlayer CreateBotPlayerObject(UserProfile profile, BotModes diff, CoordinateGenerator coordGene)
        {
            return new BotPlayer(diff, profile, coordGene);
        }

        private void PlotShipsOnAllMaps(List<BattleShip> shipList)
        {
            foreach(KeyValuePair<BasePlayer, Map> register in ActivePlayRegistry)
            {
                //Checks if a key has a map object, else it will not plot
                BasePlayer currentPlayer = register.Key;
                Map map = register.Value;

                if (map != null)
                {
                    map.PlotShips(shipList);
                    ActivePlayRegistry[currentPlayer] = map;
                }

                
            }
            
        }

        public void AttemptShot(int x, int y)
        {
            try
            {
                //Activates Player Turn
                StartPlayerTurn();

                //Shoots at chosen coordinates
                NewAttemptShot(new Coordinate(x, y));
            }
            catch(Exception e)
            {
                RaiseErrorEvent("Unkown Error Occured", e);
            }
        }

        private void RaiseErrorEvent(string msg, Exception e)
        {
            ErrorOccurred?.Invoke(this, new BattleEventArgs.ErrorEventArgs(
                msg,
                e));
        }

        private void CheckType(object given, object expected)
        {
            if (given.GetType() != expected.GetType())
            {
                throw new InputException(
                    expected,
                    given,
                    "Invalid Input has been provided");
            }
        }

        private void StartPlayerTurn()
        {
            Player player = ActivePlayRegistry.Keys
                    .OfType<Player>()
                    .FirstOrDefault();

            player.GameData.ActivateTurn();

        }

        private void NewAttemptShot(Coordinate targetedCoordinate)
        {
            //Get Active Player and its target map
            BasePlayer activePlayer = GetActivePlayer();
            Map targetMap = GetTargetMap(activePlayer);

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

        private Coordinate ResolveBotCoords(BotPlayer bot)
        {
            //Get the oppenent map with shot history
            Map targetMap = GetTargetMap(GetOppenentPlayer());

            //Update stats for the bot to use
            bot.Moves.UpdateCoordinateRegister(targetMap.HitShots, targetMap.MissedShots);

            //Return the calculated coordinate
            return bot.CalculateNextCoordinate();    
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

            //If the inactive player of the round was a bot
            if (currentInactivePlayer is BotPlayer bot)
            {
                //Generate a coordinate that the bot will use to fire
                Coordinate BotSelectedCoordinate = ResolveBotCoords(bot);

                //Fires at the player's fleet
                NewAttemptShot(BotSelectedCoordinate);
            }
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
                    firedMap.ActiveShips.Remove(shipThatWasHit);
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
}