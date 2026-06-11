using System;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public class ShipHull : HullBase
    {
        private float speedLevel;
        private int maxSpeedLevel = 3;
        private int minSpeedLevel = -1;
        private float targetSpeed;

        public override void SetTargetSpeed(Vector2 directionToPoint)
        {
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToPoint.normalized);
            if (Mathf.Abs(angleToTarget) > 120f)
            {
                speedLevel = 0;
                return;
            }
            if (Mathf.Abs(angleToTarget) < 60f)
            {
                speedLevel = Mathf.Clamp(speedLevel + 1, minSpeedLevel, maxSpeedLevel);
            }
            else
            {
                speedLevel = Mathf.Clamp(speedLevel - 1, minSpeedLevel, maxSpeedLevel);
            }
        }

        public override void AddSpeed(bool isAddition)
        {
            speedLevel = Mathf.Clamp(speedLevel + (isAddition ? 1 : -1), minSpeedLevel, maxSpeedLevel);
        }

        public override void Movement(float rotationDirection)
        {
            if (Data == null) return;
            targetSpeed = speedLevel * (Data.MaxSpeed / maxSpeedLevel);
            CurrentSpeed = MathF.Min(
                Mathf.MoveTowards(CurrentSpeed, targetSpeed, Data.Acceleration * Time.fixedDeltaTime),
                Data.MaxSpeed);
            float angle = rotationDirection * Data.RotationSpeed * Time.fixedDeltaTime;
            rigidbody2D.MoveRotation(rigidbody2D.rotation + angle);
            Vector2 nextPos = rigidbody2D.position + (Vector2)transform.up * CurrentSpeed * Time.fixedDeltaTime;
            rigidbody2D.MovePosition(nextPos);
        }
    }
}
