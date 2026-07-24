using BattleEnumCollection;
using BattleShipCollection;

namespace BattlePlayers
{
    public class BotPlayer : BasePlayer
    {
        public BotModes Difficulty { get; set; }
        public BotMoves Moves { get; set; }

        public BotPlayer(BotModes diff, UserProfile profile, CoordinateGenerator coordGene) : base(profile)
        {
            this.Difficulty = diff;
            this.Moves = new BotMoves(coordGene);
        }

        public void UpdateShotOutcome(ShotOutcome outcome, Coordinate coord)
        {
            this.Moves.CoordinateOutcome = outcome;
            this.Moves.PreviousCoordiate = coord;
        }

        public Coordinate CalculateNextCoordinate()
        {
            Coordinate chosenCoord = default;

            //Calls method based on the difficulty of bot
            if (Difficulty == BotModes.EASY)
            {
                chosenCoord = Moves.DetermineMoveEasy();
            }

            else if (Difficulty == BotModes.MEDIUM)
            {
                chosenCoord = Moves.DetermineMoveMedium();
            }

            return chosenCoord;
        }
    }

    public enum BotModes
    {
        NODIFF,
        EASY,
        MEDIUM,
        CHEATER,
        AIMODE
    }
}