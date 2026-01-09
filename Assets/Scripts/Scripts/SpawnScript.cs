using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public class SpawnScript : ScriptBase
    {
        public Vector3 Position;
        private bool _isExecuted;
        public EntityHullSetup EntityBodySetupToSpawn;
        //add code for spawning/ it will be like that with point moving
        public override bool Execute(EntityController entityController)
        {
            return true;
        }
        public override bool IsFinished(EntityController entityController) => true;
        public override bool IsExecuted(EntityController entityController) => _isExecuted;
    }
}
