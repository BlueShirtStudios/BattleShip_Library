namespace BattlePlayers
{
    public class CurrentGameData
    {
        public int Score { get; private set; }
        public PlayerStatus Status { get; set; }
        public int ShotsMade { get; set; }
        public bool IsTurn { get; set; }

        public CurrentGameData()
        {
            this.Score = 0;
            this.Status = PlayerStatus.WAITING;
            this.IsTurn = false;
            this.ShotsMade = 0;
        }

        public void UpdateScore(int score)
        {
            Score += score;
        }
        
        public void UpdateShotsMade()
        {
            ShotsMade++;
        }

        public void SetStatusToActive()
        {
            Status = PlayerStatus.ACTIVE;
        }

        public void SetStatusToEliminated()
        {
            Status = PlayerStatus.ELIMINMATED;
        }

        public void ActivateTurn()
        {
            IsTurn = true;
        }

        public void EndTurn()
        {
            IsTurn = false;
        }

        public void ResetForNewGAme()
        {
            Score = 0;
            Status = PlayerStatus.WAITING;
            IsTurn = false;
            ShotsMade = 0;
        }
      
    }

    public enum PlayerStatus
    {
        WAITING,
        ACTIVE,
        ELIMINMATED,
    }
}
