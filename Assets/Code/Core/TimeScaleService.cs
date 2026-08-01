using FK.Common;
using FK.Tulip.Data.GameEvents;
using UnityEngine;

namespace FK.Tulip.Core
{
    internal sealed class TimeScaleService : MonoBehaviour
    {
        [SerializeField, Required] private GameStateChangeEvent gameStateChangeEvent;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            gameStateChangeEvent.OnRaised += GameState_Changed;
        }

        private void OnDisable()
        {
            gameStateChangeEvent.OnRaised -= GameState_Changed;
        }

        private static void GameState_Changed(GameStateChange change)
        {
            Time.timeScale = change.NewState switch
            {
                GameState.Paused => 0f,
                _ => 1f
            };
        }
    }
}
