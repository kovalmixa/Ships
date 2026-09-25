using Assets.AI;
using Assets.Entity.AI.Interfaces;
using Assets.Entity.Hull;
using Assets.Entity.Modifiers;
using Entity.Controllers;
using Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AiDriverController : MonoBehaviour, IAiDriver, IAi
    {
        public Transform MovePoint { get; set; }
        public Transform TargetPoint { get; set; }

        [Header("References")]
        [SerializeField] private EntityController _entityController;

        private Queue<ScriptBase> _scriptQueue = new();
        private Dictionary<ScriptBase, Action> _activeScripts = new();
        private readonly List<ScriptBase> _scriptBuffer = new();
        private IAi ai;

        #region Setup & Initialization

        public void Setup(EntityController entityController)
        {
            _entityController = entityController;
            _nextCheckTime = Time.time + UnityEngine.Random.Range(0f, _checkInterval);
        }

        #endregion

        #region Public control tools

        public void SetAiType(string name) { }

        public void AddScripts(params ScriptBase[] scripts)
        {
            foreach (var script in scripts) _scriptQueue.Enqueue(script);
        }

        #endregion

        #region Control Loop

        public void UpdateControl()
        {
            if (!_entityController) return;

            ActivateScripts();
            TryActivateNextScript();

            if (!_isAiSleeping)
            {
                MoveControl();
                RotateControl();
                AttackControl();
            }

            ProcessBrainAndDistanceCheck();
        }

        private void AttackControl()
        {
            if (TargetPoint == null) return;
        }

        private void RotateControl()
        {
            if (TargetPoint == null) return;
            _entityController.Hull.RotateEquipment(TargetPoint.position);
        }

        private void MoveControl()
        {
            PointMovement();
        }

        private void PointMovement()
        {
            HullBase hullBase = _entityController.Hull;
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

        private void ActivateScripts()
        {
            _scriptBuffer.Clear();
            _scriptBuffer.AddRange(_activeScripts.Keys);

            foreach (var script in _scriptBuffer)
            {
                script.Execute(_entityController);
                if (script.IsFinished(_entityController))
                {
                    _activeScripts[script]?.Invoke();
                    _activeScripts.Remove(script);
                }
            }
        }

        private void TryActivateNextScript()
        {
            if (_scriptQueue.Count == 0) return;

            ScriptBase nextScript = _scriptQueue.Peek();
            bool isSameTypeActive = false;

            foreach (var active in _activeScripts.Keys)
            {
                if (active.GetType() == nextScript.GetType())
                {
                    isSameTypeActive = true;
                    break;
                }
            }

            if (!isSameTypeActive)
            {
                _scriptQueue.Dequeue();
                _activeScripts.Add(nextScript, OnScriptCompleted);
                nextScript.Execute(_entityController);
            }
        }

        private void OnScriptCompleted()
        {
        }

        #endregion

        #region AI Brain & Distance Management

        [Header("AI Brain & Optimization")]
        [SerializeField] private float _checkInterval = 0.5f;
        private float _nextCheckTime;
        private bool _isAiSleeping = false;
        private Transform _playerTransform;

        private void ProcessBrainAndDistanceCheck()
        {
            if (Time.time < _nextCheckTime) return;
            _nextCheckTime = Time.time + _checkInterval + UnityEngine.Random.Range(-0.05f, 0.05f);
            UpdateSleepAndLodState();
            if (!_isAiSleeping) EvaluateStrategy();
        }

        private void UpdateSleepAndLodState()
        {
            if (_playerTransform == null)
            {
                FindPlayerReference();
                if (_playerTransform == null) return;
            }

            float attackRange = _entityController.GetTotalLifetimeStat(StatType.MaxRange);
            float visionRange = attackRange * 1.5f; //moderated by ai type
            float sqrDistance = (_playerTransform.position - transform.position).sqrMagnitude;
            float sqrVisionRange = visionRange * visionRange;
            float sqrAttackRange = attackRange * attackRange;

            if (sqrDistance > sqrVisionRange)
            {
                _isAiSleeping = true;
                _checkInterval = 1.0f;
            }
            else if (sqrDistance > sqrAttackRange)
            {
                _isAiSleeping = false;
                _checkInterval = 0.35f;
            }
            else
            {
                _isAiSleeping = false;
                _checkInterval = 0.15f;
            }
        }

        private void EvaluateStrategy()
        {
            // Логика выбора цели, смены поведения и т.д.
        }

        private void FindPlayerReference()
        {
            // Ваша система поиска игрока
            // _playerTransform = GameObjectHandler.PlayerTransform;
        }

        #endregion
    }
}