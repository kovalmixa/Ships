using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public class MoveToScript : ScriptBase
    {
        [SerializeField] public Transform Target;
        private bool _isExecuted;
        public override bool Execute(Entity.EntityBody entityBody)
        {
            if (entityBody.EntityController.IsPlayer) return false;
            if (Target == null) return false;
            entityBody.EntityController.SetPointToMove(Target);
            _isExecuted = true;
            return true;
        }

        public override bool IsExecuted(Entity.EntityBody entityBody) => _isExecuted;
        public override bool IsFinished(Entity.EntityBody entityBody)
        {
            float threshold = entityBody.CollisionSize.y + 0.5f;
            CircleCollider2D area = Target.GetComponent<CircleCollider2D>();
            if (area) return Vector3.Distance(entityBody.transform.position, Target.position) < area.radius + threshold;
            return Vector3.Distance(entityBody.transform.position, Target.position) < threshold;
        }
        #if UNITY_EDITOR
                private void OnDrawGizmos()
                {
                    if (Target != null)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(transform.position, Target.position);
                        Gizmos.DrawSphere(Target.position, 0.1f);
                    }
                }
        #endif
    }
}