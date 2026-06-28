using BattleShipCollection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

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

        //Test Run
        bool run = true;
        Console.WriteLine("---- BATTLE SHIPS ----");
        while (run)
        {
            DisplayMainMenu();
            int option = Convert.ToInt32(Console.ReadLine());
            switch (option)
            {
                case 1:
                    //Get input
                    Console.WriteLine("---- Please enter where you want to fire ---");
                    Console.Write("x: ");
                    int x = Convert.ToInt32(Console.ReadLine());
                    Console.Write("y: ");
                    int y = Convert.ToInt32(Console.ReadLine());

                    //Fire the shot
                    battleMap.FireShot(x, y);

                    //Display the result
                    Console.WriteLine(battleMap.ShotResult);
                    Console.WriteLine();

                    break;

                case 2:
                    Console.WriteLine("---- MAP ----");
                    DisplayMap(visualMap, battleMap);
                    break;

                case 3:
                    Console.WriteLine("---- Master MAP ----");
                    DisplayMasterMap(visualMap, battleMap);
                    break;
            }
        }
        

    }

    static private void DisplayMainMenu()
    {
        
        Console.WriteLine("[1] Fire Shot");
        Console.WriteLine("[2] View Map");
        Console.WriteLine();
    }

    static private void DisplayMap(CoordStates[,] visualMap, Map battleMap)
    {
        //Populate Array
        for (int x = 0; x < battleMap.XSize; x++)
        {
            for (int y = 0; y < battleMap.YSize; y++)
            {
                visualMap[x, y] = battleMap.GetCoordState(battleMap.ConvertIntsToCoord(x + 1, y + 1));
            }
        }

        //Display the content of the array + colom numbers
        for (int r = 0; r < visualMap.GetLength(0); r++)
        {
            Console.Write($"{r + 1,-5}");

            for (int c = 0; c < visualMap.GetLength(1); c++)
            {
                Console.Write($"{visualMap[r, c],-7}");
            }
            Console.WriteLine();
        }

        //Dsiplay row numbers
        Console.Write($"{"",-5}");

        for (int c = 0; c < visualMap.GetLength(1); c++)
        {
            Console.Write($"{c + 1,-7}");
        }
        Console.WriteLine();
    }

    static private void DisplayMasterMap(CoordStates[,] visualMap, Map battleMap)
    {
        //Populate Array
        for (int x = 0; x < battleMap.XSize; x++)
        {
            for (int y = 0; y < battleMap.YSize; y++)
            {
                if (battleMap.DoesCoordinatePairExist(battleMap.ConvertIntsToCoord(x + 1, y + 1)))
                {
                    visualMap[x, y] = CoordStates.SHIP;
                }
                else
                {
                    visualMap[x, y] = battleMap.GetCoordState(battleMap.ConvertIntsToCoord(x + 1, y + 1));
                }
                    
            }
        }

        //Display the content of the array + colom numbers
        for (int r = 0; r < visualMap.GetLength(0); r++)
        {
            Console.Write($"{r + 1,-5}");

            for (int c = 0; c < visualMap.GetLength(1); c++)
            {
                Console.Write($"{visualMap[r, c],-7}");
            }
            Console.WriteLine();
        }

        //Dsiplay row numbers
        Console.Write($"{"",-5}");

        for (int c = 0; c < visualMap.GetLength(1); c++)
        {
            Console.Write($"{c + 1,-7}");
        }
        Console.WriteLine();
    }
}
