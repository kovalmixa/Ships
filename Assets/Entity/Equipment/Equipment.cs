using System.Collections;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Actions;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject
    {
        public Entity Entity { get; set; }
        public HullEquipmentProperties HullEquipmentProperties { set; get; }
        private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer
        {
            get => _equipmentContainer;
            set
            {
                _equipmentContainer = value;
                string[] texturePaths = _equipmentContainer.Graphics.Textures;
                StartCoroutine(SetupTextureLayers(texturePaths));
                
            }
        }
        public int LayerIndex;
        private IEnumerator SetupTextureLayers(string[] texturePaths)
        {
            yield return StartCoroutine(SetupLayersCoroutine(texturePaths));
            foreach (Transform child in LayersAnchor.GetComponentsInChildren<Transform>())
            {
                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.sortingOrder += LayerIndex + 1;
            }
            Quaternion rotation = transform.rotation;
            rotation.z = HullEquipmentProperties.Rotation;
            transform.rotation = rotation;
        }
        private void Awake()
        {
            IsComplexCollision = true;
        }
        public new string Type
        {
            get => EquipmentContainer.General.SizeType;
            set => EquipmentContainer.General.SizeType = value;
        }
        public void Rotate(float angle)
        {
            if (!CanRotate()) return;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            Quaternion rotationStep = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _equipmentContainer.Physics.RotationSpeed * Time.deltaTime
            );
            float resultWorldAngle = NormalizeAngle(rotationStep.eulerAngles.z);
            float hullRotation = NormalizeAngle(Entity.transform.eulerAngles.z);
            float baseRotation = NormalizeAngle(HullEquipmentProperties.Rotation);
            float resultLocalAngle = NormalizeAngle(resultWorldAngle - hullRotation - baseRotation);
            Vector2 sector = HullEquipmentProperties.RotationSector;
            float minSector = NormalizeAngle(sector.x);
            float maxSector = NormalizeAngle(sector.y);
            if (IsAngleWithinSector(resultLocalAngle, minSector, maxSector))
            {
                transform.rotation = rotationStep;
            }
        }
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
        private bool IsAngleWithinSector(float angle, float min, float max) => min <= angle && angle <= max;
        public bool CanRotate()
        {
            if (HullEquipmentProperties == null) return false;
            return HullEquipmentProperties.RotationSector != null;
        }
        public void Activate(Transform target)
        {
            if (!IsActivationWithinSector()) return;
            Debug.Log("activated");
            foreach (var activation in EquipmentContainer.OnActivate)
            {
                ActionContext actionContext = FormActionContext(activation, target.position);
                ActionHandler.Execute(activation.Type, actionContext);
            }
        }
        private bool IsActivationWithinSector()
        {
            Vector2 fireSector = HullEquipmentProperties.FireSector;
            float angle = NormalizeAngle(transform.rotation.z);
            return fireSector.x <= angle && angle <= fireSector.y;
        }
        private ActionContext FormActionContext(ActivationContainer activation, Vector3 position)
        {
            ActionContext actionContext = new();
            actionContext.ObjectId = activation.Projectile;
            actionContext.Source = GetComponent<GameObject>();
            actionContext.TargetPosition = position;
            //actionContext.AmountValue = ?? // для добавления значения к абилкам или хп, мп и так далее
            return actionContext;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
    }
}
