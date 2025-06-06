using System.Collections.Generic;
using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public class AreaScript : ScriptBase
    {
        [SerializeField] public List<MonoBehaviour> Scripts;
        [SerializeField] public CircleCollider2D Collider;
        private List<IScript> _scripts = new();
        private bool _isExecuted;

        public List<IScript> GetScripts() => _scripts;
        private void Awake()
        {
            foreach (MonoBehaviour script in Scripts)
            {
                if (script is IScript s)
                    _scripts.Add(s);
                else
                    Debug.LogWarning("Assigned scriptBehaviour does not implement IScript!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Entity.Entity entity = other.GetComponent<Entity.Entity>();
            if (entity) Execute(entity);
        }
        public override bool Execute(Entity.Entity entity)
        {
            if (_scripts.Count == 0)
            {
                Debug.LogWarning("Script list is empty!");
                return false;
            }
            foreach (IScript _script in _scripts)
            {
                _script.Execute(entity);
            }
            _isExecuted = true;
            return true;
        }
        public override bool IsFinished(Entity.Entity entity) => true;
        public override bool IsExecuted(Entity.Entity entity) => _isExecuted;
    }
}