using BattleShipCollection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

class BattleMathConsoleVersion
{
    static void Main()
    {
        BattleShipCollection.BattleEngine battleEngine = new BattleEngine();
        battleEngine.CreateGameMap(1, 1);
        battleEngine.Initialize1WayGame();
        battleEngine.AttemptShot(0, 0);
        Console.WriteLine(battleEngine.MessageFromPlayerResult);
      
    }
}
