using System.Collections.Generic;
using Assets.InGameMarkers.Scripts;
using UnityEngine;

namespace Assets.Entity.AI
{
    public class AiController : MonoBehaviour, IEntityController, IAi
    {
        private Transform _movePoint;
        private Transform _targetPoint;
        public Vector2 Size;
        public Queue<IScript> ScriptList = new();
        private IAi _ai;
        public void SetAiType(string name) { }
        public void UpdateControl(EntityBody entityBody)
        {
            if (!entityBody) return;
            ActivateScripts(entityBody);
            MoveControl(entityBody);
            RotateControl(entityBody);
            AttackControl(entityBody);
        }
        private void AttackControl(EntityBody entityBody)
        {
        }
        private void RotateControl(EntityBody entityBody)
        {
        }
        private void MoveControl(EntityBody entityBody)
        {
            PointMovement(entityBody);
        }
        private void PointMovement(EntityBody entityBody)
        {
            if (_movePoint == null) return;
            Vector2 directionToPoint = _movePoint.transform.position - entityBody.transform.position;
            if (directionToPoint.magnitude < Size.y) return; 
            float angleToTarget = Vector2.SignedAngle(entityBody.transform.up, directionToPoint.normalized);
            float rotationDirection = -Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
            entityBody.SpeedLevel = Mathf.Clamp(entityBody.SpeedLevel + 1, entityBody.MinSpeedLevel, entityBody.MaxSpeedLevel);
            entityBody.Movement(rotationDirection);
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
        private void ActivateScripts(EntityBody entityBody)
        {
            if (ScriptList.Count == 0) return;
            if (!IsExecuted(entityBody))
            {
                ScriptList.Peek().Execute(entityBody);
                return;
            }
            if (IsScriptActive(entityBody)) return;
            ScriptList.Dequeue();
            if (ScriptList.Count == 0) return;
            ScriptList.Peek().Execute(entityBody);
        }

        private bool IsScriptActive(EntityBody entityBody) => !ScriptList.Peek().IsFinished(entityBody);
        private bool IsExecuted(EntityBody entityBody) => ScriptList.Peek().IsExecuted(entityBody);
    }
}