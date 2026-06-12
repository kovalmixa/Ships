using UnityEngine;

namespace Assets.Entity.Hull
{
    public class GroundHullBase : HullBase
    {
        public override void SetTargetSpeed(Vector2 directionToPoint)
        {
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToPoint.normalized);
            float targetSpeed = Mathf.Clamp(directionToPoint.magnitude, 0, data.maxSpeed);
            if (Mathf.Abs(angleToTarget) < 90f)
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, data.acceleration * Time.deltaTime);
            else
                currentSpeed = Mathf.MoveTowards(currentSpeed, -targetSpeed, data.acceleration * Time.deltaTime);
        }
        public override void AddSpeed(bool isAddition)
        {
            currentSpeed = currentSpeed + (isAddition ? 1 : -1);
        }

        public override void Movement(float rotationDirection)
        {
            throw new System.NotImplementedException();
        }
    }
}
