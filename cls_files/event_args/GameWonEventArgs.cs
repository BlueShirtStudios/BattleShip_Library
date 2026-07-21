namespace BattleEventArgs
{
    public class GameWonEventArgs : AGameEndEventArgs
    {
        public GameWonEventArgs(int cScore, int cAmountShots, string cEntity) : base(cAmountShots, cScore, cEntity)
        {
            //IDK will be expanded
        }
    }
}