using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public class SpawnScript : ScriptBase
    {
        public Vector3 Position;
        private bool _isExecuted;
        public Entity.Entity EntityToSpawn;
        //add code for spawning/ it will be like that with point moving
        public override bool Execute(Entity.Entity entity)
        {
            return true;
        }
        public override bool IsFinished(Entity.Entity entity) => true;
        public override bool IsExecuted(Entity.Entity entity) => _isExecuted;
    }
}
