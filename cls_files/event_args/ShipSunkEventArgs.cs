namespace BattleEventArgs
{
    public class ShipSunkArgs : EventArgs
    {
        public string ShipName { get; }
        public int Score { get; }
        public int ShotMadeBeforeSunk { get; }

        public ShipSunkArgs(string cShipName, int cScore, int cShotMadeBeforeSunk)
        {
            this.ShipName = cShipName;
            this.Score = cScore;
            this.ShotMadeBeforeSunk = cShotMadeBeforeSunk;
        }
    }
}