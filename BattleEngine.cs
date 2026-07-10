using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using fileHandlerComponents;

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
            if (GameMode == GameModes.ONEWAY)
            {
                try
                {
                    //The client gets to fire
                    BotMap.FireShot(x, y);
                    MessageFromPlayerResult = BotMap.ShotResult;
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
                    BotMap.FireShot(x, y);
                    MessageFromPlayerResult = BotMap.ShotResult;

                    //Bot fires back
                    Coordinate botChosenCoords = CoordGenerator.GenerateNewCoordinate();
                    PlayerMap.FireShot(botChosenCoords.X, botChosenCoords.Y);
                    MessageFromBotResult = PlayerMap.ShotResult;
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

        public enum GameModes
        {
            ONEWAY,
            TWOWAY
        }
    }
}

