using Entity.Controllers;
using UnityEngine;

namespace Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
        protected bool isExecuted;

        public abstract bool Execute(Entity.Controllers.EntityController entityController);

        public bool IsExecuted(Entity.Controllers.EntityController entityController) => isExecuted;

        public abstract bool IsFinished(Entity.Controllers.EntityController entityController);
    }
}