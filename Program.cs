using BattleShipCollection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

class BattleMathConsoleVersion
{
    static void Main()
    {
        //Create an instance of the object
        BattleShipCollection.BattleEngine battleEngine = new BattleEngine();

        //Initialize the game
        //Specify the map size
        battleEngine.CreateGameMap(2, 3);

        //Select the mode you want to play - it takes string of integer
        battleEngine.SelectGameMode("1Way");

        //Add Ships
        battleEngine.AddShipToMap("Small", 1, 2);

        //Call the initialze method
        battleEngine.InitializeGame();

        //Attempt Shots and Display Messages
        battleEngine.AttemptShot(1, 1);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);

        battleEngine.AttemptShot(1, 2);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);

        battleEngine.AttemptShot(1, 3);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);

        battleEngine.AttemptShot(2, 1);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);

        battleEngine.AttemptShot(2, 2);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);

        battleEngine.AttemptShot(2, 3);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);
      
    }
}
