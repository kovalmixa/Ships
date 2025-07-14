using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public class WaitScript : ScriptBase
    {
        private float waitTime;
        private float timer = 0f;

        public WaitScript(float waitTime)
        {
            this.waitTime = waitTime;
        }

        public override bool Execute(Entity.EntityBody entityBody)
        {
            timer += Time.deltaTime;
            return timer >= waitTime;
        }

        public override bool IsExecuted(Entity.EntityBody entityBody)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsFinished(Entity.EntityBody entityBody)
        {
            throw new System.NotImplementedException();
        }
    }
}