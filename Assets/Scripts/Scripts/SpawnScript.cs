using Entity.Controllers;
using UnityEngine;

namespace Scripts
{
    public class SpawnScript : ScriptBase
    {
        public Vector3 Position;
        public Entity.Controllers.EntityController entitToSpawm;
        //add code for spawning/ it will be like that with point moving
        public override bool Execute(Entity.Controllers.EntityController entityController)
        {
            return true;
        }
        public override bool IsFinished(Entity.Controllers.EntityController entityController) => true;
    }
}
