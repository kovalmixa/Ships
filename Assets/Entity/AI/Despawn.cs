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
        private GameObject _entity;
        private void Awake()
        {
            _entity = transform.parent.gameObject;
            _objectPool = SceneNodesHandler.GetNode("ObjectPools").transform.Find("EntityPool").GetComponent<ObjectPoolHandler>();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var entityBody = other.GetComponent<EntityBody>();
            if (entityBody != null && entityBody.EntityController != null && entityBody.EntityController.IsPlayer)
            {
                _objectPool.Return(_entity);
            }
        }

    }
}
