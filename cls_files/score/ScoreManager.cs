using BattleShipCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleScoreComponents;
using BattlePlayers;
using BattleEventArgs;
using BattleEnumCollection;

namespace BattleScoreComponents
{
    public class ScoreManager
    {
        public double FinalScore { get; private set; }
        public double BonusMultiplier { get; set; }
        public Dictionary<int, RoundScore> ScoreHistory = new();
        public int DefaultScore { get; }
        public BasePlayer Owner { get; }


        public ScoreManager(BattleEngine engine, int defaultScore, BasePlayer owner)
        {
            this.FinalScore = 0;
            this.BonusMultiplier = 1;
            engine.ShotAttempt += DetermineRoundScore;
            this.DefaultScore = defaultScore;
            this.Owner = owner;
        }

        public void DetermineRoundScore(object? sender, ShotResultEventArgs e)
        {
            //Checks if the owner of this score object is the active shooter/player
            if (Owner.Equals(e.Shooter))
            {
                //Create a default round object
                RoundScore scoreDetails = new RoundScore(DefaultScore, BonusMultiplier, new Coordinate(e.X, e.Y), e.Result);

                //Check if we can apply bonus score
                CheckSpecialConditions(scoreDetails);

                //Add it to our list of entries
                ScoreHistory.Add(ScoreHistory.Count + 1, scoreDetails);
            }
        }

         private void CheckSpecialConditions(RoundScore roundScore)
         {
            //Looks for consectutive hits
            int hitStreak = GetHitStreak();

            //Update our multiplier
            UpdateMultiplier(hitStreak);

            //Update our score based on the multiplier
            FinalScore = roundScore.Score * BonusMultiplier;

            //Assign the final score to or session record
            roundScore.Score += FinalScore;

            //Reset the multiplier to one for the next shot
            ResetMultilpier();
         }

        private void UpdateMultiplier(int factor)
        {
            int currentMulti = factor > 0 ? factor : 1;
            BonusMultiplier = BonusMultiplier * currentMulti;
        }

        private int GetHitStreak()
        {
            //Initialize
            int streak = 0;
            ShotOutcome prev_outcome = ShotOutcome.NONE;

            //Go through each entry in the history and determine how many shots were consectutive hits
            foreach (KeyValuePair<int, RoundScore> entry in ScoreHistory)
            {
                //If the outcome was a hit
                if ((entry.Value.Outcome == BattleEnumCollection.ShotOutcome.HIT))
                {
                    //Increase the streak for a hit
                    streak++;
                    if (prev_outcome == ShotOutcome.HIT)
                    {
                        streak++;
                    }
                }
                else
                {
                    //If it is not a hit, it resets the streak counter
                    streak = 0;
                }

                    //Stores Previous Entry state to check for the consectutive hits
                    prev_outcome = entry.Value.Outcome;
            }

            //Returns the determined streak counter
            return streak;
        }

        private void ResetMultilpier()
        {
            BonusMultiplier = 1;
        }

    }
}
