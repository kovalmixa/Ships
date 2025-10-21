using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class ExplosionAction : ActionBase
    {
        [SerializeField] private EffectAction _effectAction;
        public override void Execute(GameObject source, Vector3 targetPos)
        {
            Debug.Log("boom");
            _effectAction.Execute(source, targetPos);
        }
    }
}
