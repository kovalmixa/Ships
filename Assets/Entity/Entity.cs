using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Entity
{
    public class Entity : InGameObject
    {
        public EntityController EntityController;
        public EntityContainer EntityData = new();
        public GameObject EquipmentPrefab;
        private List<GameObject> _equipments = new();
        private const int InLayerComponentsLimit = 10;
        public float MaxSpeed = 5f;
        public float Acceleration = 3f;
        public float RotationSpeed = 60f;
        private float _targetSpeed;
        public float SpeedLevel
        {
            get => Speed;
            set => Speed = value;
        }
        private int _maxSpeedLevel = 3;
        public int MaxSpeedLevel => _maxSpeedLevel;
        private int _minSpeedLevel = -1;
        public int MinSpeedLevel => _minSpeedLevel;
        public IEnumerator StartSetupHullLayers(HullContainer hull)
        {
            foreach (Transform child in LayersAnchor)
                Destroy(child.gameObject);

            EntityData.HullData = hull;
            string[] texturePaths = hull.Graphics.Textures;
            yield return StartCoroutine(SetupHullLayers(texturePaths));
        }
        private IEnumerator SetupHullLayers(string[] texturePaths)
        {
            yield return StartCoroutine(SetupLayersCoroutine(texturePaths));
            SetupEquipmentsFrames();
        }
        private void SetupEquipmentsFrames()
        {
            if (EquipmentPrefab == null)
            {
                Debug.LogError("EquipmentPrefab не назначен!");
                return;
            }
            HullEquipmentProperties[][] weaponProperties = EntityData.HullData.Equipments;
            if (weaponProperties == null || weaponProperties.Length == 0) return;
            for (int i = 0; i < weaponProperties.Length; i++)
            {
                GameObject layer = Layers[i];
                if (layer == null) continue;
                HullEquipmentProperties[] innerArray = weaponProperties[i];
                for (int j = 0; j < innerArray.Length; j++)
                {
                    HullEquipmentProperties hullEquipmentProperties = weaponProperties[i][j];
                    GameObject layerGo = Instantiate(EquipmentPrefab, layer.transform);
                    layerGo.name = $"Equipment_{j}";
                    float ppu = layer.GetComponent<Sprite>() == null ? 100 : layer.GetComponent<Sprite>().pixelsPerUnit;
                    layerGo.transform.localPosition = Vector3.zero + (Vector3)hullEquipmentProperties.Position / ppu;
                    SpriteRenderer layerRenderer = layer.GetComponent<SpriteRenderer>();
                    if (layerRenderer != null) layerRenderer.sortingOrder = i * InLayerComponentsLimit;
                    Equipment.Equipment equipment = layerGo.GetComponent<Equipment.Equipment>();
                    equipment.HullEquipmentProperties = hullEquipmentProperties;
                    equipment.LayerIndex = i * InLayerComponentsLimit;
                    _equipments.Add(layerGo);
                }
            }
        }
        public bool SetEquipment(EquipmentContainer equipmentContainer, int index)
        {
            Equipment.Equipment equipment = _equipments[index].GetComponent<Equipment.Equipment>();
            //if (equipment.Type == equipmentContainer.)
            //логика соответствия слота с тем оружием, которое пихается туда
            equipment.EquipmentContainer = equipmentContainer;
            return false;
        }
        public void Movement(float rotationDirection)
        {
            switch (Type)
            {
                case "Sea":
                {
                    _targetSpeed = SpeedLevel * (MaxSpeed / _maxSpeedLevel);
                    CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, _targetSpeed, Acceleration * Time.deltaTime);
                    transform.Rotate(Vector3.forward, -rotationDirection * RotationSpeed * Time.deltaTime);
                    transform.Translate(Vector3.up * CurrentSpeed * Time.deltaTime, Space.Self);
                    break;
                }
            }
        }
        public void RotateEquipment(float angle)
        {
            foreach (var eq in _equipments)
            {
                eq.GetComponent<Equipment.Equipment>().Rotate(angle);
            }
        }
    }
}
