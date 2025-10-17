using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Entity.AI
{
    public class Despawn : MonoBehaviour
    {
        public float DespawnDistance
        {
            get => GetComponent<CircleCollider2D>().radius;
            set => GetComponent<CircleCollider2D>().radius = value;
        }
        private ObjectPoolHandler _objectPool;
        [SerializeField] private GameObject _entity;
        private void Awake()
        {
            _objectPool = SceneNodesHandler.GetPoolHandler("EntityPool");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController != null && entityController != null && entityController.IsPlayer)
            {
                _objectPool.Return(_entity);
            }
        }

    }
}
