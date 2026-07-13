using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileInstance : MonoBehaviour
    {
        private ProjectileDefinition _definition;
        private Vector2 _direction;
        private InterractionContext _context;

        public void Setup(InterractionContext interractionContext)
        {
            _context = interractionContext;
            Vector3 direction = (_definition.targetPosition - _definition.startPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.SetPositionAndRotation(_definition.startPosition, Quaternion.identity);
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);   
        }


        public void Tick()
        {

        }
    }
}
