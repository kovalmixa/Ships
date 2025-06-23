using UnityEngine;

namespace Assets.InGameMarkers.Actions
{
    public class ActionContext
    {
        public GameObject Target;
        public GameObject Source;
        public Vector3? TargetPosition;
        public string ObjectId { get; set; }
        public int? AmountValue { get; set; }
    }
}
