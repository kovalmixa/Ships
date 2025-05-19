using Assets.Entity;
using UnityEngine;

public class MoveToScript : MonoBehaviour, IScript
{
    [SerializeField] public Transform Target;
    [SerializeField] private float pointReachThreshold = 0.5f;

    public bool Execute(Entity entity)
    {
        if (Target == null) return true;
        Vector2 directionToPoint = (Vector2)(Target.position - entity.transform.position);
        if (directionToPoint.magnitude < pointReachThreshold)
        {
            return true;
        }
        float angleToTarget = Vector2.SignedAngle(entity.transform.up, directionToPoint.normalized);
        float rotationDirection = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        entity.SpeedLevel = Mathf.Clamp(entity.SpeedLevel + 1, entity.MinSpeedLevel, entity.MaxSpeedLevel);
        entity.Movement(rotationDirection);
        return false;
    }
}