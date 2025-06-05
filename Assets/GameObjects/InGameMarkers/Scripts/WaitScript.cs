using UnityEngine;

namespace Assets.GameObjects.InGameMarkers.Scripts
{
    public class WaitScript : ScriptBase
    {
        private float waitTime;
        private float timer = 0f;

        public WaitScript(float waitTime)
        {
            this.waitTime = waitTime;
        }

        public override bool Execute(Entity.Entity entity)
        {
            timer += Time.deltaTime;
            return timer >= waitTime;
        }

        public override bool IsExecuted(Entity.Entity entity)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsFinished(Entity.Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}