using System.Collections.Generic;
using Assets.Entity;
using Assets.Entity.AI.Interfaces;
using Assets.Entity.Hull;
using Entity.Controllers.GenericController;
using Scripts;
using UnityEngine;

namespace Entity.Controllers.AI
{
    public class AiController : MonoBehaviour, IEntityController, IAi
    {
        private Transform movePoint;
        private Transform targetPoint;
        public Queue<ScriptBase> Scripts;
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
        }

        private void RotateControl(EntityController entityController)
        {
        }

        private void MoveControl(EntityController entityController)
        {
            // тут от ии зависит продолжать скрипт или переходит в тактику
            PointMovement(entityController);
        }

        private void PointMovement(EntityController entityController)
        {
            HullBase hullBase = entityController.hull;
            if (movePoint == null)
            {
                hullBase.SetTargetSpeed(Vector2.zero);
                return;
            }
            Vector2 directionToPoint = movePoint.position - hullBase.transform.position;
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

        public void SetMovementPoint(Transform target) => movePoint = target;

        public void SetTargetPoint(Transform target) => targetPoint = target;

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