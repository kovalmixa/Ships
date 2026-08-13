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
        private bool _isInputBlocked = false;
        private CameraController _cameraController;

        public CameraController CameraController
        {
            get
            {
                if (_cameraController != null) return _cameraController;
                _cameraController = SceneController.GetNodeByType<CameraController>();
                return _cameraController;
            }
        }

        private readonly Dictionary<KeyCode, KeyAction> _keyBinds = new()
    {
        { KeyCode.Mouse0, new KeyAction(WeaponType.Primary) },
        { KeyCode.Mouse1, new KeyAction(WeaponType.Secondary) },
    };

        private void Awake()
        {
            for (int i = 0; i <= 9; i++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
                _keyBinds[key] = i == 1 ? new KeyAction(AbilityType.Heal) : new KeyAction(AbilityType.None);
            }

            GUIHandler.OnInputBlockedStateChanged += OnGUIBlocked;
        }

        private void OnDestroy()
        {
            GUIHandler.OnInputBlockedStateChanged -= OnGUIBlocked;
        }

        private void OnGUIBlocked(bool isBlocked) => _isInputBlocked = isBlocked;

        public void UpdateControl(EntityController controller)
        {
            if (!controller) return;
            Vector2 worldPos = CameraController.CursorPosition;
            CameraControl();

            if (_isInputBlocked)
            {
                controller.Move(0, 0);
                return;
            }
            MoveControl(controller);
            controller.AimAt(worldPos);
            ActionControls(controller, worldPos);
        }

        private void CameraControl()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f) CameraController.Instance.ZoomTo(scroll);
            CameraController.ManualMove(Input.GetMouseButtonDown(2), Input.GetMouseButton(2));
        }

        private void MoveControl(EntityController controller)
        {
            float rotationInput = 0f;
            float accel = 0f;

            if (!_isInputBlocked)
            {
                if (Input.GetKeyDown(KeyCode.W)) accel = 1f;
                else if (Input.GetKeyDown(KeyCode.S)) accel = -1f;
                rotationInput = -Input.GetAxis("Horizontal");
            }
            controller.Move(accel, rotationInput);
        }

        private void ActionControls(EntityController controller, Vector2 targetPos)
        {
            if (_isInputBlocked) return;
            foreach (var entry in _keyBinds)
            {
                bool isPressed = entry.Key == KeyCode.Mouse0 ? Input.GetMouseButton(0)
                    : entry.Key == KeyCode.Mouse1 ? Input.GetMouseButton(1)
                    : Input.GetKey(entry.Key);

                if (isPressed) controller.ExecuteAction(entry.Value, targetPos);
            }
        }
    }
}