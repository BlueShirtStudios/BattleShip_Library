# BattleShip_Library

A lightweight, highly customizable C# library that handles the core logic of Battleship, so you don't have to build it from scratch.

Whether you want to build a mini-game inside a larger project, create an educational tool for students, or develop an entirely new spin on the classic Battleship formula, this library takes care of the grid management, ship placement, and hit/miss tracking for you.

Table of Contents
Architectural View
Current Features
Roadmap & Upcoming Features
Requirements
Installation
How to Use
Contributing
License
🏗️ Architectural View
The Battle Engine — The central entry point of the library. It orchestrates all internal components and exposes a clean, simplified API for managing game loops and grid logic.
Unlimited Customization — Total control over the battlefield. Developers can dynamically define grid dimensions and specify custom ship types, lengths, and quantities.
Dynamic Rule Engine (coming soon) — Built with extensibility in mind. Future updates will let you inject custom win/loss conditions, modify firing mechanics, or enforce turn/shot limits to create unique gameplay modes.
🚀 Current Features
Single-Player Ready — Initialize a game grid and deploy a fleet for a player to interact with in just a few lines of code.
Player vs. AI — Play against a basic AI opponent that returns fire after each of your turns.
Targeting System — Built-in coordinate validation with hit/miss/sink state tracking.
Stat Tracker — Persistent or session-based tracking to monitor player performance, accuracy, and win rates across multiple matches.
🗺️ Roadmap & Upcoming Features
 Intelligent AI Counter-Attacks — Upgrade the AI opponent from basic/random targeting to a smarter strategy that hunts down and targets discovered ships.
 Custom Rule Injection — Interfaces to easily script custom game modifiers (e.g., ammo limits, radar sweeps).
 Local High Scores — Extended serialization for the Stat Tracker to persist scores between sessions.
📋 Requirements
.NET 6.0 or later
C# 10.0 or later
📦 Installation

Package will be available via NuGet once the library reaches its stable v1.0 release.

bash
dotnet add package BattleShip_Library

In the meantime, you can reference the project directly:

bash
git clone https://github.com/<your-username>/BattleShip_Library.git

Note: The library's namespace is BattleShipCollection — import it with using BattleShipCollection; as shown below.

💻 How to Use

The BattleEngine is initialized with an EngineConfig, which defines the game mode, difficulty, grid size, and fleet composition. Gameplay is driven through events, so you subscribe to the outcomes you care about (shots, sunk ships, wins/losses, errors) and then fire shots with AttemptShot(x, y).

csharp
using BattleShipCollection;

class Program
{
    static void Main()
    {
        // Create the engine and its configuration
        var battleEngine = new BattleEngine();
        var engineCfg = new EngineConfig(
                            "2WAY",   // Game mode
                            "Medium", // Bot difficulty
                            2,        // Grid max size (x)
                            3);       // Grid max size (y)

        // Add a ship to the fleet roster
        engineCfg.AddShipToRoster("Small", 1, 2);

        // Pass the configuration to the engine
        battleEngine.InitializeGame(engineCfg);

        // Subscribe to game events
        battleEngine.ShotAttempt += (sender, e) =>
            Console.WriteLine($"Shot ({e.X}, {e.Y}) Outcome: {e.Result}");

        battleEngine.ShipSunk += (sender, e) =>
            Console.WriteLine($"{e.ShipName} was sunk in {e.ShotMadeBeforeSunk} shots (Score: {e.Score})");

        battleEngine.GameWon += (sender, e) =>
            Console.WriteLine($"{e.Entity} won! Shots: {e.TotalShots}, Score: {e.TotalScore}");

        battleEngine.GameLoose += (sender, e) =>
            Console.WriteLine($"{e.Entity} lost. Shots: {e.TotalShots}, Score: {e.TotalScore}");

        battleEngine.ErrorOccurred += (sender, e) =>
            Console.WriteLine($"{e.Msg}: {e.Error}");

        battleEngine.GameEnd += (sender, e) =>
        {
            Console.WriteLine("The game has ended!");
            Environment.Exit(0);
        };

        // Fire shots at coordinates
        battleEngine.AttemptShot(1, 1);
        battleEngine.AttemptShot(1, 2);
        battleEngine.AttemptShot(2, 2);
    }
}
Available Events
Event	Fires When	Key Properties
ShotAttempt	A shot is fired at a coordinate	X, Y, Result
ShipSunk	A ship's last section is hit	ShipName, ShotMadeBeforeSunk, Score
GameWon	The game ends in a win	Entity, TotalShots, TotalScore
GameLoose	The game ends in a loss	Entity, TotalShots, TotalScore
GameEnd	The game session concludes (win or loss)	—
ErrorOccurred	An invalid action or internal error occurs	Msg, Error

Note: Full API documentation covering EngineConfig options, difficulty levels, and roster rules will be published as the library approaches v1.0. A project wiki with a full enum reference and technical deep-dives is planned as well.

Ship placement: Calling AddShipToRoster is all you need to do — the engine handles placing the ship on the grid internally. You don't need to specify coordinates or manage placement logic yourself.

🤝 Contributing

Contributions, issues, and feature requests are welcome. Feel free to check the issues page if you'd like to help out.

📄 License

This project is licensed under the MIT License.

