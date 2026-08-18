using BattleEnumCollection;
using BattleShipCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleScoreComponents
{
    public class RoundScore
    {
        public double Score { get; set; }
        public double AppliedMultiplier { get; set; }
        public Coordinate TargetedCoordinate { get; }
        public ShotOutcome Outcome { get; }

        public RoundScore(double cScore, double cMul, Coordinate cTarget, ShotOutcome cOutcome)
        {
            this.Score = cScore;
            this.AppliedMultiplier = cMul;
            this.TargetedCoordinate = cTarget;
            this.Outcome = cOutcome;
        }
    }

    
}
