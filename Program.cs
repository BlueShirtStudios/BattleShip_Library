using BattleShipCollection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

class BattleMathConsoleVersion
{
    static void Main()
    {
        //This is a test file so that I can test as I develop. This will be changed to a library when it is in a more working condition
        //Create an instance of the object
        BattleShipCollection.BattleEngine battleEngine = new BattleEngine();

        //Initialize the game
        //Specify the map size
        battleEngine.CreateGameMap(2, 3);

        //Select the mode you want to play - it takes string of integer
        battleEngine.SelectGameMode("2Way");

        //Add Ships
        battleEngine.AddShipToMap("Small", 1, 2);

        //Subscribe to the events
        battleEngine.ShotAttempt += (sender, e) =>
        {
            Console.WriteLine($"Shot ({e.X}, {e.Y}) Outcome: {e.Result}");
            Console.WriteLine("");
        };

        battleEngine.GameWon += (sender, e) =>
        {
            Console.WriteLine($"{e.Entity} WON THE GAME!!!");
            Console.WriteLine($"MATCH STATS: Shots Made: {e.TotalShots}, Score: {e.TotalScore}");
            Console.WriteLine("");
        };

        battleEngine.GameLoose += (sender, e) =>
        {
            Console.WriteLine($"{e.Entity} Lost.");
            Console.WriteLine($"MATCH STATS: Shots Made: {e.TotalShots}, Score: {e.TotalScore}");
            Console.WriteLine("");
        };

        battleEngine.ShipSunk += (sender, e) =>
        {
            Console.WriteLine($"{e.ShipName} WAS SUNK!!!");
            Console.WriteLine($"Shots Made to Sink: {e.ShotMadeBeforeSunk}, Score: {e.Score}");
            Console.WriteLine("");
        };

        battleEngine.GameEnd += OnGameEnd;
        

        //Call the initialze method
        battleEngine.InitializeGame();

        //Attempt Shots
        battleEngine.AttemptShot(1, 1);
        battleEngine.AttemptShot(1, 2);
        battleEngine.AttemptShot(1, 3);
        battleEngine.AttemptShot(2, 1);
        battleEngine.AttemptShot(2, 2); 
        battleEngine.AttemptShot(2, 3);
        
    }

    private static void OnGameEnd(object? sender, EventArgs e)
    {
        Console.WriteLine("The game has ended!");
    }
}
