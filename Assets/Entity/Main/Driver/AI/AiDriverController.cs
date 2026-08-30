using Assets.AI;
using Assets.Entity.AI.Interfaces;
using Assets.Entity.Hull;
using Entity.Controllers;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public class AiDriverController : MonoBehaviour, IAiDriver, IAi
    {
        public Transform MovePoint { get; set; }
        public Transform TargetPoint { get; set; }

        private Queue<ScriptBase> _scriptQueue = new();
        private Dictionary<ScriptBase, Action> _activeScripts = new();
        private IAi ai;

        #region Public control tools

        public void SetAiType(string name) { }

        public void AddScripts(params ScriptBase[] scripts)
        {
            foreach (var script in scripts) _scriptQueue.Enqueue(script);
        }

        #endregion

        #region Control

        public void UpdateControl(EntityController entityController)
        {
            if (!entityController) return;
            ActivateScripts(entityController);
            TryActivateNextScript(entityController);

            MoveControl(entityController);
            RotateControl(entityController);
            AttackControl(entityController);
        }

        private void AttackControl(EntityController entityController)
        {
            if (TargetPoint == null) return;
            //entityController.totalAbbilitiesController.Invoke(_targetPoint.position, AbilityType.FirePrimary);
        }

        private void RotateControl(EntityController entityController)
        {
            if (TargetPoint == null) return;
            entityController.hull.RotateEquipment(TargetPoint.position);
        }

        private void MoveControl(EntityController entityController)
        {
            PointMovement(entityController);
        }

        private void PointMovement(EntityController entityController)
        {
            HullBase hullBase = entityController.hull;
            if (MovePoint == null)
            {
                hullBase.SetTargetSpeed(Vector2.zero);
                return;
            }
            Vector2 directionToPoint = MovePoint.position - hullBase.transform.position;
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

            if (Mathf.Abs(angleToTarget) < 10f) hullBase.SetTargetSpeed(directionToPoint);
            else hullBase.SetTargetSpeed(Vector2.zero);
        }

        #endregion

        #region Script handler

        private void ActivateScripts(EntityController entityController)
        {
            var activeList = _activeScripts.Keys.ToList();

            foreach (var script in activeList)
            {
                script.Execute(entityController);
                if (script.IsFinished(entityController))
                {
                    _activeScripts[script]?.Invoke();
                    _activeScripts.Remove(script);
                }
            }
        }

        private void TryActivateNextScript(EntityController entityController)
        {
            if (_scriptQueue.Count == 0) return;

            ScriptBase nextScript = _scriptQueue.Peek();
            bool isSameTypeActive = _activeScripts.Keys.Any(active => active.GetType() == nextScript.GetType());

            if (!isSameTypeActive)
            {
                _scriptQueue.Dequeue();
                _activeScripts.Add(nextScript, OnScriptCompleted);
                nextScript.Execute(entityController);
            }
        }

        private void OnScriptCompleted()
        {
        }

        #endregion
    }
}
