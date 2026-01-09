using UnityEngine;

namespace Assets.Entity.Hull
{
    public class GroundHullBase : HullBase
    {
        public override void SetTargetSpeed(Vector2 directionToPoint)
        {
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToPoint.normalized);
            float targetSpeed = Mathf.Clamp(directionToPoint.magnitude, 0, Data.MaxSpeed);
            if (Mathf.Abs(angleToTarget) < 90f)
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, Data.Acceleration * Time.deltaTime);
            else
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, -targetSpeed, Data.Acceleration * Time.deltaTime);
        }
        public override void AddSpeed(bool isAddition)
        {
            CurrentSpeed = CurrentSpeed + (isAddition ? 1 : -1);
        }

        public override void Movement(float rotationDirection)
        {
            throw new System.NotImplementedException();
        }
    }
}
