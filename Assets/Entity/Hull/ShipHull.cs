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
            if (data == null) return;
            _targetSpeed = _speedLevel * (data.maxSpeed / _maxSpeedLevel);
            currentSpeed = MathF.Min(
                Mathf.MoveTowards(currentSpeed, _targetSpeed, data.acceleration * Time.fixedDeltaTime),
                data.maxSpeed);
            float angle = rotationDirection * data.rotationSpeed * Time.fixedDeltaTime;
            rigidBody2D.MoveRotation(rigidBody2D.rotation + angle);
            Vector2 nextPos = rigidBody2D.position + (Vector2)transform.up * currentSpeed * Time.fixedDeltaTime;
            rigidBody2D.MovePosition(nextPos);
        }
    }
}
