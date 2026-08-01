using FK.Common;
using FK.Tulip.Core;
using FK.Tulip.Data.GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FK.Tulip.Input
{
    [DefaultExecutionOrder(-10)]
    public sealed class InputService : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private GameStateChangeEvent gameStateChangeEvent;

        [Header("Config")]
        [SerializeField] private bool activateHeroAtStart;
        [SerializeField] private bool logDeviceEvents = true;
        [SerializeField] private bool logControlSwaps = true;

        private InputActionMap heroMap;
        private InputActionMap uiMap;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            heroMap = InputSystem.actions.FindActionMap("Hero");
            uiMap = InputSystem.actions.FindActionMap("UI");
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += InputSystem_DeviceChanged;
            gameStateChangeEvent.OnRaised += GameState_Changed;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= InputSystem_DeviceChanged;
            gameStateChangeEvent.OnRaised -= GameState_Changed;
        }

        private void Start()
        {
            ToggleControls(activateHeroAtStart);
        }

        private void GameState_Changed(GameStateChange change)
        {
            ToggleControls(change.NewState is GameState.Playing);
        }

        private void ToggleControls(bool isHeroActive)
        {
            if (isHeroActive)
                ActivateHeroControls();
            else
                ActivateUIControls();
        }

        private void ActivateHeroControls()
        {
            if (logControlSwaps)
                Log.Info($"{logPrefix} Switching to Hero controls");

            heroMap.Enable();
            uiMap.Disable();
        }

        private void ActivateUIControls()
        {
            if (logControlSwaps)
                Log.Info($"{logPrefix} Switching to UI controls");

            heroMap.Disable();
            uiMap.Enable();
        }

        private void InputSystem_DeviceChanged(InputDevice device, InputDeviceChange change)
        {
            if (device is Mouse or Keyboard) return;
            if (change is InputDeviceChange.Removed or InputDeviceChange.Added) return;

            if (logDeviceEvents)
                Log.Info($"{logPrefix} {change}: {device.displayName}");
        }

        private static readonly string logPrefix = $"[{nameof(InputService)}]";
    }
}
