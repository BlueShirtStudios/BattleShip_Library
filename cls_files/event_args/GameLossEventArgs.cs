using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleEventArgs
{
    public class GameLossEventArgs : AGameEndEventArgs
    {
        public GameLossEventArgs(int cScore, int cTotalShots, string cEntity) : base(cTotalShots, cScore, cEntity)
        {
            //Will be expanded
        }
    }
}
