using Assets.AI;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers;
using AI;
using UnityEngine;

namespace Scripts
{
    public class MoveToScript : ScriptBase
    {
        [SerializeField] protected Transform Target;
        public override bool Execute(EntityController entityController)
        {
            if (Target == null) return false;
            var ai = GameObjectHandler.GetAI(entityController);
            if (ai != null)
            {
                ai.SetMovementPoint(Target);
                isExecuted = true;
                return true;
            }
            isExecuted = true;
            return true;
        }

        public override bool IsFinished(EntityController entityController)
        {
            float threshold = 3.5f;
            CircleCollider2D area = Target.GetComponent<CircleCollider2D>();
            if (area) return Vector3.Distance(entityController.transform.position, Target.position) < area.radius + threshold;
            return Vector3.Distance(entityController.transform.position, Target.position) < threshold;
        }
        #if UNITYEDITOR
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