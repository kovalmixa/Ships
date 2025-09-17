using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class ExplosionAction : ActionBase
    {
        public override void Execute(GameObject source, Vector3 targetPos)
        {
            Debug.Log("boom");
            //ActivationHandler.Execute("effect", actionContext);
        }
    }
}
