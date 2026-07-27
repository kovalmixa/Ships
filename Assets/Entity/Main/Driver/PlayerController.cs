using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Controllers
{
    public enum ActionCategory { None, Weapon, Ability }

    public struct KeyAction
    {
        public ActionCategory Category;
        public int ActionId;

        public KeyAction(WeaponType weapon)
        {
            Category = ActionCategory.Weapon;
            ActionId = (int)weapon;
        }

        public KeyAction(AbilityType ability)
        {
            Category = ActionCategory.Ability;
            ActionId = (int)ability;
        }
    }

    public class PlayerController : MonoBehaviour, IDriver
    {
        private CameraController _cameraController;
        public CameraController CameraController
        {
            get
            {
                if (_cameraController != null) return _cameraController;
                _cameraController = SceneController.GetNodeByType<CameraController>();
                return _cameraController;
            }
            private set => _cameraController = value;
        }

        private readonly Dictionary<KeyCode, KeyAction> _keyBinds = new()
        {
            { KeyCode.Mouse0, new KeyAction(WeaponType.Primary) },
            { KeyCode.Mouse1, new KeyAction(WeaponType.Secondary) },
        };

        #region Setup

        private void Awake()
        {
            for (int i = 0; i <= 9; i++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
                //_keyCodeActivations[key] = i == 1 ? AbilityType.Heal : AbilityType.None;
            }
        }

        public void SetupControl()
        {

        }

        #endregion

        #region Update Control

        public void UpdateControl(EntityController controller)
        {
            if (!controller) return;
            Vector2 worldPos = CameraController.CursorPosition;

            CameraControl(controller, worldPos);
            MoveControl(controller);

            controller.hull.RotateEquipment(worldPos);
            KeyWordControls(controller, worldPos);
        }
        
        private void CameraControl(EntityController controller, Vector2 position)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f) CameraController.Instance.ZoomTo(scroll);
            CameraController.ManualMove(Input.GetMouseButtonDown(2), Input.GetMouseButton(2));
        }

        private void MoveControl(EntityController controller)
        {
            Assets.Entity.Hull.HullBase hullBase = controller.hull;
            if (Input.GetKeyDown(KeyCode.W)) hullBase.AddSpeed(true);
            else if (Input.GetKeyDown(KeyCode.S)) hullBase.AddSpeed(false);
            float rotationInput = Input.GetAxis("Horizontal");
            hullBase.Movement(-rotationInput);
        }

        private void KeyWordControls(EntityController controller, Vector2 position)
        {

            foreach (var entry in _keyBinds)
            {
                if (Input.GetKey(entry.Key))
                {
                    if (entry.Value.Category == ActionCategory.Weapon)
                        controller.totalAbbilitiesController.Invoke(position, (WeaponType)entry.Value.ActionId);
                    else if (entry.Value.Category == ActionCategory.Ability)
                        controller.totalAbbilitiesController.Invoke(position, (AbilityType)entry.Value.ActionId);
                }

                bool held = entry.Key == KeyCode.Mouse0 ? Input.GetMouseButton(0)
                    : entry.Key == KeyCode.Mouse1 ? Input.GetMouseButton(1)
                    : Input.GetKey(entry.Key);
                if (!held) continue;
            }
        }
        #endregion
    }
}
