using BattleShipCollection;

namespace BattlePlayers
{
    public class BotPlayer : BasePlayer
    {
        public BotModes Difficulty { get; set; }
        public Coordinate PreviousCoordiate { get; set; }
        public ShotOutcome CoordinateOutcome { get; set; }

        public BotPlayer(BotModes diff, UserProfile profile) : base(profile)
        {
            this.Difficulty = diff;
        }

        public void UpdateShotOutcome(ShotOutcome outcome, Coordinate coord)
        {
            PreviousCoordiate = coord;
            CoordinateOutcome = outcome;
        }

        public Coordinate CalculateNextCoordinate()
        {
            return new Coordinate(0,0);
        }
    }

    public enum BotModes
    {
        EASY,
        MEDIUM,
        CHEATER,
        AIMODE
    }
}