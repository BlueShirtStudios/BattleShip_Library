using BattleEnumCollection;
using BattleExceptions;
using BattlePlayers;
using BattleShipCollection;
using System.ComponentModel;

public class EngineConfig
{
    public GameModes GameMode { get; set; }
    public BotModes Difficulty { get; set; }
    public int xSize { get; set; }
    public int ySize { get; set; }

    public  List<BattleShip> RequesteShips { get; }

    public EngineConfig(string cGameMode, string cBotMode, int cXSixe, int cYSize)
    {
        this.GameMode = ToGameModes(cGameMode);
        this.Difficulty = ToBotModes(cBotMode);
        this.xSize = cXSixe;
        this.ySize = cYSize;
        this.RequesteShips = new();
    }

    private GameModes ToGameModes(string strMode)
    {
        //Extract from input which is used to determine the mode
        int mode = Convert.ToInt32(strMode[0].ToString());
        GameModes selectedMode = GameModes.NOWAY;

       
        //Assign the game mode
        switch (mode)
        {
            case 1:
                selectedMode = GameModes.ONEWAY;
                break;

            case 2:
                selectedMode = GameModes.TWOWAY;
                break;

            default:
                throw new ModeNotSupportedError("Entered mode is not supported. Could not initialize session.");
            
        }

        return selectedMode;
       
    }

    private BotModes ToBotModes(string pdiff)
    {
        if (GameMode != GameModes.TWOWAY)
        {
            throw new BotIn1WayPlayException("Cannot create a bot when 1 way play is active.");
        }

        //Get first letter of diff
        char cDiff = Char.ToLower(pdiff[0]);
        BotModes diff = BotModes.NODIFF;

        switch (cDiff)
        {
            case 'e':
                diff = BotModes.EASY;
                break;

            case 'm':
                diff = BotModes.MEDIUM;
                break;

            default:
                diff = BotModes.NODIFF;
                break;
        }

        return diff;

    }

    public void AddShipToRoster(string name, int width, int length)
    {
        RequesteShips.Add(new BattleShip(name, width, length));
    }
}