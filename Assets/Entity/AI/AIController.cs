using System;
using System.Collections.Generic;
using Assets.InGameMarkers.Scripts;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
        public void UpdateControl(Entity entity)
        {
            if (!entity) return;
            ActivateScripts(entity);
            MoveControl(entity);
            RotateControl(entity);
            AttackControl(entity);
        }
        private void AttackControl(Entity entity)
        {
        }
        private void RotateControl(Entity entity)
        {
        }
        private void MoveControl(Entity entity)
        {
            PointMovement(entity);
        }
        private void PointMovement(Entity entity)
        {
            if (_movePoint == null) return;
            Vector2 directionToPoint = _movePoint.transform.position - entity.transform.position;
            if (directionToPoint.magnitude < Size.y) return; 
            float angleToTarget = Vector2.SignedAngle(entity.transform.up, directionToPoint.normalized);
            float rotationDirection = -Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
            entity.SpeedLevel = Mathf.Clamp(entity.SpeedLevel + 1, entity.MinSpeedLevel, entity.MaxSpeedLevel);
            entity.Movement(rotationDirection);
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
        private void ActivateScripts(Entity entity)
        {
            if (ScriptList.Count == 0) return;
            if (!IsExecuted(entity))
            {
                ScriptList.Peek().Execute(entity);
                return;
            }
            if (IsScriptActive(entity)) return;
            ScriptList.Dequeue();
            if (ScriptList.Count == 0) return;
            ScriptList.Peek().Execute(entity);
        }

        private bool IsScriptActive(Entity entity) => !ScriptList.Peek().IsFinished(entity);
        private bool IsExecuted(Entity entity) => ScriptList.Peek().IsExecuted(entity);
    }
}