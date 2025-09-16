using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class ExplosionAction : ActionBase, IGameAction
    {
        public override void Execute(GameObject source, Vector3 targetPos)
        {
            Debug.Log("boom");
            //ActivationHandler.Execute("effect", actionContext);
        }
    }
}
