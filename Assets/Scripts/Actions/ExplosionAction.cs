using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class ExplosionAction : MonoBehaviour, IGameAction
    {
        public bool IsPassive { get; set; } = true;
        public void Execute(GameObject source, Vector3 targetPos)
        {
            Debug.Log("boom");
            //ActivationHandler.Execute("effect", actionContext);
        }
    }
}
