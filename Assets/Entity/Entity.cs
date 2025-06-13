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
            HullEquipmentProperties[][] weaponProperties = EntityData.HullData.Equipments;
            if (weaponProperties == null || weaponProperties.Length == 0) return;
            for (int i = 0; i < weaponProperties.Length; i++)
            {

                GameObject layer = Layers[i];
                HullEquipmentProperties[] innerArray = weaponProperties[i];
                for (int j = 0; j < innerArray.Length; j++)
                {
                    HullEquipmentProperties equipment = weaponProperties[i][j];

                    if (EquipmentPrefab == null)
                    {
                        Debug.LogError("EquipmentPrefab не назначен!");
                        return;
                    } 
                    GameObject layerGo = Instantiate(EquipmentPrefab, layer.transform);
                    layerGo.name = $"Equipment_{j}";
                    layerGo.transform.localPosition = Vector3.zero + (Vector3)equipment.Position / 100;
                    _equipments.Add(layerGo);
                }
            }
        }

        public bool SetupEquipment(EquipmentContainer equipmentContainer, int index)
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
    }
}
