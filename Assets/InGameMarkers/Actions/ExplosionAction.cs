using UnityEngine;

namespace Assets.InGameMarkers.Actions
{
    public class ExplosionAction : IGameAction
    {
        public bool IsPassive { get; set; } = true;
        public void Execute(ActionContext actionContext)
        {
            Debug.Log("boom");
            ActivationHandler.Execute("effect", actionContext);
        }
    }
}
