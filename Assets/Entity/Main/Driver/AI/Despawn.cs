using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Entity.Controllers.AI
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
            _objectPool = SceneController.GetPoolHandler("EntityPool");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController != null && entityController != null && GameObjectHandler.IsPlayer(entityController))
            {
                _objectPool.Return(_entity);
            }
        }

        public void SetEntity(GameObject entity) => entity = entity;
    }
}
