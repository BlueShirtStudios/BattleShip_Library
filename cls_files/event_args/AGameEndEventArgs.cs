namespace BattleEventArgs
{
    public abstract class AGameEndEventArgs : EventArgs
    {
        public int TotalShots { get; }
        public int TotalScore { get; }
        public string Entity { get; }

        protected AGameEndEventArgs(int cTotalShots, int cTotalScore, string cEntity)
        {
            this.TotalShots = cTotalShots;
            this.TotalScore = cTotalScore;
            this.Entity = cEntity;
        }

    }
}