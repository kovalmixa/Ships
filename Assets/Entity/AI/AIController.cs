using System.Collections.Generic;
using Assets.Entity.AI.Interfaces;
using Assets.Entity.Interfaces;
using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.Entity.AI
{
    public class AiController : MonoBehaviour, IEntityController, IAi
    {
        private Transform _movePoint;
        private Transform _targetPoint;
        public Queue<IScript> ScriptList = new();
        private IAi _ai;
        public void SetAiType(string name) { }
        public void UpdateControl(EntityController entityController)
        {
            if (!entityController) return;
            ActivateScripts(entityController);
            MoveControl(entityController);
            RotateControl(entityController);
            AttackControl(entityController);
        }
        private void AttackControl(EntityController entityController)
        {
        }
        private void RotateControl(EntityController entityController)
        {
        }
        private void MoveControl(EntityController entityController)
        {
            PointMovement(entityController);
        }
        private void PointMovement(EntityController entityController)
        {
            if (_movePoint == null) return;
            Hull.Hull hull = entityController.Hull;
            Vector2 directionToPoint = _movePoint.transform.position - hull.transform.position;
            if (directionToPoint.magnitude < 3) return; 
            float angleToTarget = Vector2.SignedAngle(hull.transform.up, directionToPoint.normalized);
            float rotationDirection = -Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
            hull.SpeedLevel = Mathf.Clamp(hull.SpeedLevel + 1, hull.MinSpeedLevel, hull.MaxSpeedLevel);
            hull.Movement(rotationDirection);
        }
        public void SetupAreaScripts(GameObject[] scriptAreaSets)
        {
            throw new System.NotImplementedException();
        }
        public void SetupScripts(IScript[] scriptPointSets)
        {
            throw new System.NotImplementedException();
        }
        public void SetMovementPoint(Transform target) => _movePoint = target;
        public void SetTargetPoint(Transform target) => _targetPoint = target;
        private void ActivateScripts(EntityController entityController)
        {
            if (ScriptList.Count == 0) return;
            if (!IsExecuted(entityController))
            {
                ScriptList.Peek().Execute(entityController);
                return;
            }
            if (IsScriptActive(entityController)) return;
            ScriptList.Dequeue();
            if (ScriptList.Count == 0) return;
            ScriptList.Peek().Execute(entityController);
        }

        private bool IsScriptActive(EntityController entityController) => !ScriptList.Peek().IsFinished(entityController);
        private bool IsExecuted(EntityController entityController) => ScriptList.Peek().IsExecuted(entityController);
    }
}