using Assets.Entity.AI.Interfaces;
using Assets.Entity.Controllers.AI;
using Assets.Entity.Hull;
using Assets.Handlers.Enums;
using Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Controllers.AI
{
    public class AiController : MonoBehaviour, IAIDriver, IAi
    {
        private Transform _movePoint;
        private Transform _targetPoint;
        public Queue<ScriptBase> Scripts { get; set; }
        private IAi ai;
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
            if (_targetPoint == null) return;
            entityController.abbilitiesController.Invoke(_targetPoint.position, AbilityType.FirePrimary);
        }

        private void RotateControl(EntityController entityController)
        {
            if (_targetPoint == null) return;
            entityController.hull.RotateEquipment(_targetPoint.position);
        }

        private void MoveControl(EntityController entityController)
        {
            PointMovement(entityController);
        }

        private void PointMovement(EntityController entityController)
        {
            HullBase hullBase = entityController.hull;
            if (_movePoint == null)
            {
                hullBase.SetTargetSpeed(Vector2.zero);
                return;
            }
            Vector2 directionToPoint = _movePoint.position - hullBase.transform.position;
            float distance = directionToPoint.magnitude;
            if (distance < 3f)
            {
                hullBase.SetTargetSpeed(Vector2.zero);
                return;
            }
            Vector2 forward = hullBase.transform.up;
            float angleToTarget = Vector2.SignedAngle(forward, directionToPoint.normalized);
            float rotationDirection = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
            hullBase.Movement(rotationDirection);

            if (Mathf.Abs(angleToTarget) < 10f)
                hullBase.SetTargetSpeed(directionToPoint);
            else
                hullBase.SetTargetSpeed(Vector2.zero);
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
            if (Scripts.Count == 0) return;
            if (!IsExecuted(entityController))
            {
                Scripts.Peek().Execute(entityController);
                return;
            }
            if (IsScriptActive(entityController)) return;
            Scripts.Dequeue();
            if (Scripts.Count == 0) return;
            Scripts.Peek().Execute(entityController);
        }

        private bool IsScriptActive(EntityController entityController) => !Scripts.Peek().IsFinished(entityController);

        private bool IsExecuted(EntityController entityController) => Scripts.Peek().IsExecuted(entityController);
    }
}
