using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Scripts;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using HullWeaponProperties = Assets.Entity.DataContainers.HullWeaponProperties;

namespace Assets.Entity
{
    public class Entity : InGameObject
    {
        public EntityController EntityController;
        public EntityContainer EntityData = new();

        private List<GameObject> _weapons = new();

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

        public void SetupHullLayers(HullContainer hull)
        {
            EntityData.HullData = hull;
            string[] texturePaths = hull.Graphics.Textures;
            StartCoroutine(SetupHullLayers(texturePaths));
        }
        private IEnumerator SetupHullLayers(string[] texturePaths)
        {
            yield return StartCoroutine(SetupLayersCoroutine(texturePaths));
            SetupWeapons();
        }
        private void SetupWeapons()
        {
            HullWeaponProperties[][] weaponProperties = EntityData.HullData.Weapons;
            if (weaponProperties == null || weaponProperties.Length == 0) return;
            for (int i = 0; i < weaponProperties.Length; i++)
            {
                GameObject layer = Layers[i];
                HullWeaponProperties[] innerArray = weaponProperties[i];
                for (int j = 0; j < innerArray.Length; j++)
                {
                    GameObject weaponGo = new GameObject($"Weapon_{j}");
                    HullWeaponProperties weapon = weaponProperties[i][j];
                    weaponGo.transform.SetParent(layer.transform, false);
                    weaponGo.transform.localPosition = weapon.Position;
                }
            }
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
