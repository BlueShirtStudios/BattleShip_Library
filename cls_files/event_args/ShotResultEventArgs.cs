using BattleShipCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleEnumCollection;
using BattlePlayers;

namespace BattleEventArgs
{
    public class ShotResultEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public ShotOutcome Result { get; }

        public BasePlayer Shooter{ get; }

        public ShotResultEventArgs(ShotOutcome cResult, int cX, int cY, BasePlayer cPlayer)
        {
            this.X = cX;
            this.Y = cY;
            this.Result = cResult;
            this.Shooter = cPlayer;
        }
    }
}
