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
        private ObjectPoolHandler objectPool;
        [SerializeField] private GameObject entity;
        private void Awake()
        {
            objectPool = SceneNodesHandler.GetPoolHandler("EntityPool");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController != null && entityController != null && entityController.isPlayer)
            {
                objectPool.Return(entity);
            }
        }

        public void SetEntity(GameObject entity) => entity = entity;
    }
}
