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
            _targetSpeed = _speedLevel * (Data.MaxSpeed / _maxSpeedLevel);
            CurrentSpeed = MathF.Min(
                Mathf.MoveTowards(CurrentSpeed, _targetSpeed, Data.Acceleration * Time.fixedDeltaTime),
                Data.MaxSpeed);
            float angle = -rotationDirection * Data.RotationSpeed * Time.fixedDeltaTime;
            Rigidbody2D.MoveRotation(Rigidbody2D.rotation + angle);
            Vector2 nextPos = Rigidbody2D.position + (Vector2)transform.up * CurrentSpeed * Time.fixedDeltaTime;
            Rigidbody2D.MovePosition(nextPos);
        }
    }
}
