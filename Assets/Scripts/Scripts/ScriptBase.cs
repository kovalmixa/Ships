using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
        protected bool isExecuted;

        public abstract bool Execute(EntityController entityController);

        public bool IsExecuted(EntityController entityController) => isExecuted;

        public abstract bool IsFinished(EntityController entityController);
    }
}