using Entity.Controllers.GenericController;
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

        public override bool Execute(EntityController entityController)
        {
            timer += Time.deltaTime;
            isExecuted = true;
            return true;
        }

        public override bool IsFinished(EntityController entityController) => timer >= waitTime;
    }
}