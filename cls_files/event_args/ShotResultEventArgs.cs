using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleEventArgs
{
    public class ShotResultEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public string Result { get; }

        public ShotResultEventArgs(string cResult, int cX, int cY)
        {
            this.X = cX;
            this.Y = cY;
            this.Result = cResult;
        }
    }
}
