using Assets.Entity.Modifiers;
using System;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public class ShipHull : HullBase
    {
        private float _speedLevel;
        private int _maxSpeedLevel = 3;
        private int _minSpeedLevel = -1;
        private float _targetSpeed;

        public override void SetTargetSpeed(Vector2 directionToPoint)
        {
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToPoint.normalized);
            if (Mathf.Abs(angleToTarget) > 120f)
            {
                _speedLevel = 0;
                return;
            }
            if (Mathf.Abs(angleToTarget) < 60f)
            {
                _speedLevel = Mathf.Clamp(_speedLevel + 1, _minSpeedLevel, _maxSpeedLevel);
            }
            else
            {
                _speedLevel = Mathf.Clamp(_speedLevel - 1, _minSpeedLevel, _maxSpeedLevel);
            }
        }

        public override void AddSpeed(bool isAddition)
        {
            _speedLevel = Mathf.Clamp(_speedLevel + (isAddition ? 1 : -1), _minSpeedLevel, _maxSpeedLevel);
        }

        public override void Movement(float rotationDirection)
        {
            var maxMoveSpeed = GetLifetimeStat(StatType.MaxMoveSpeed);
            var acceleration = GetLifetimeStat(StatType.Acceleration);
            var rotationSpeed = GetLifetimeStat(StatType.RotationSpeed);
            _targetSpeed = _speedLevel * (maxMoveSpeed / _maxSpeedLevel);
            currentSpeed = MathF.Min(
                Mathf.MoveTowards(currentSpeed, _targetSpeed, acceleration * Time.fixedDeltaTime),maxMoveSpeed);

            float angle = rotationDirection * rotationSpeed * Time.fixedDeltaTime;
            float newAngle = rigidBody2D.rotation + (rotationDirection * rotationSpeed * Time.fixedDeltaTime);
            rigidBody2D.MoveRotation(newAngle);

            Vector2 forwardDirection = Quaternion.Euler(0, 0, newAngle) * Vector2.up;
            Vector2 nextPos = rigidBody2D.position + forwardDirection * (currentSpeed * Time.fixedDeltaTime);
            rigidBody2D.MovePosition(nextPos);
        }
    }
}
