using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public class SpawnScript : ScriptBase
    {
        public Vector3 Position;
        private bool _isExecuted;
        public Entity.EntityBody EntityBodyToSpawn;
        //add code for spawning/ it will be like that with point moving
        public override bool Execute(Entity.EntityBody entityBody)
        {
            return true;
        }
        public override bool IsFinished(Entity.EntityBody entityBody) => true;
        public override bool IsExecuted(Entity.EntityBody entityBody) => _isExecuted;
    }
}
