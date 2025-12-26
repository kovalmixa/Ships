using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public class WaitScript : ScriptBase
    {
        private bool _isExecuted;
        private float _waitTime;
        private float _timer;

        public WaitScript(float waitTime)
        {
            this._waitTime = waitTime;
        }

        public override bool Execute(EntityController entityController)
        {
            _timer += Time.deltaTime;
            _isExecuted = true;
            return true;
        }

        public override bool IsExecuted(EntityController entityController) => _isExecuted;

        public override bool IsFinished(EntityController entityController) => _timer >= _waitTime;
    }
}