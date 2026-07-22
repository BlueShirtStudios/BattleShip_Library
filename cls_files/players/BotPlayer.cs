namespace BattlePlayers
{
    public class BotPlayer : BasePlayer
    {
        public BotModes Difficulty { get; set; }

        public BotPlayer(BotModes diff, UserProfile profile) : base(profile)
        {
            this.Difficulty = diff;
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