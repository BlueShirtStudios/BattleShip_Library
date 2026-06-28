using BattleShipCollection;
using System.Runtime.InteropServices;

class BattleMathConsoleVersion
{
    static void Main()
    {
        //Code for testing, not final implemetation
        //Creaet Map
        Map battleMap = new Map(10, 10);

        //Create Ships
        BattleShip carrierShip = new BattleShip("Carrier", 1, 5);
        BattleShip battleShip = new BattleShip("Battleship", 1, 4);
        BattleShip destroyer = new BattleShip("Destroyer", 1, 3);
        BattleShip submarine = new BattleShip("Submarine", 1, 3);
        BattleShip patrolBoat = new BattleShip("Patrol Boat", 1, 2);

        //Add ships to ships to be placed
        battleMap.AddShip(carrierShip);
        battleMap.AddShip(battleShip);
        battleMap.AddShip(destroyer);
        battleMap.AddShip(submarine);
        battleMap.AddShip(patrolBoat);

        //Place them on the board
        battleMap.PlotShips();

        //Build a visual representation of the board
        CoordStates[,] visualMap = new CoordStates[10, 10];
        for (int x = 0; x < battleMap.XSize; x++)
        {
            for (int y = 0; y < battleMap.YSize; y++)
            {
                visualMap[x, y] = battleMap.GetCoordState(battleMap.ConvertIntsToCoord(x, y));
            }
        }

        string gridRow;
        for (int r = 0; r < visualMap.GetLength(0); r++)
        {
            gridRow = "";
            for (int c = 0; c < visualMap.GetLength(1); c++)
            {
                gridRow += $"{visualMap[r, c]} ";
            }
            Console.WriteLine(gridRow);
        }
    }
}
