using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.InGameMarkers.Actions
{
    internal class PositionAction:IGameAction
    {
        public bool IsPassive { get; set; }
        public void Execute(ActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
