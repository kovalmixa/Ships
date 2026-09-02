using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using Entity.Controllers;

namespace Scripts
{
    public class NextLocationArea : ScriptBase
    {
        public string LocationName;
        
        public override bool Execute(EntityController entityController)
        {
            if (!GameObjectHandler.IsPlayer(entityController) || isExecuted) return false;
            SceneController.Instance.NextLocation(LocationName).Forget();
            isExecuted = true;
            return true;
        }

        public override bool IsFinished(EntityController entityController) => false;
    }
}
