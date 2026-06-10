using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
        protected bool _isExecuted;

        public abstract bool Execute(EntityController entityController);

        public bool IsExecuted(EntityController entityController) => _isExecuted;

        public abstract bool IsFinished(EntityController entityController);
    }
}