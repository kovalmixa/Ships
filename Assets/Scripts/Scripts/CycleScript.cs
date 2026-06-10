using Assets.Common;
using Entity.Controllers.GenericController;
using Scripts;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Scripts
{
    public class CycleScript : ScriptBase
    {
        [SerializeField] private ScriptBase[] scripts;
        [SerializeField] private CycleTimer timer = new();

        private EntityController entityController = null;

        private void Start()
        {
            timer.SetAction(new Action(() => activateSubScripts()));
        }

        public override bool Execute(EntityController entityController)
        {
            if (entityController == null) return false;
            this.entityController = entityController;
            timer.Launch();
            return true;
        }

        public override bool IsFinished(EntityController entityController) => timer.IsOver();

        private void activateSubScripts()
        {
            if (scripts == null) return;
            foreach (var script in scripts)
                if (script != null) script.Execute(entityController);
        }
    }
}