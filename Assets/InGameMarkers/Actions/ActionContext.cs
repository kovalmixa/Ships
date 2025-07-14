using Assets.Entity;
using Assets.Entity.DataContainers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.InGameMarkers.Actions
{
    public class ActionContext
    {
        public Vector2 Position;
        public GameObject Source;
        public Vector3? TargetPosition;
        public string ObjectId { get; set; }
        public float? AmountValue { get; set; }

        public ActionContext(ActivationContainer activation, Vector3 position, GameObject gameObject)
        {
            ObjectId = activation.Projectile;
            Source = gameObject;
            TargetPosition = position;
            Position = activation.Position;
            AmountValue = activation.Amount;
        }
    }
}
