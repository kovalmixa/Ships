using Entity.Controllers;
using UnityEngine;

namespace Scripts
{
    public class WaitScript : ScriptBase
    {
        private float waitTime;
        private float timer;

        public WaitScript(float waitTime)
        {
            this.waitTime = waitTime;
        }

        public override bool Execute(Entity.Controllers.EntityController entityController)
        {
            timer += Time.deltaTime;
            isExecuted = true;
            return true;
        }

        public override bool IsFinished(Entity.Controllers.EntityController entityController) => timer >= waitTime;
    }
}