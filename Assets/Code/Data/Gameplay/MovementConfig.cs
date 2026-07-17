using UnityEngine;

namespace FK.Tulip.Data.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/Character Movement")]
    public class MovementConfig : ScriptableObject
    {
        [Range(0f, 100f)] public float maxSpeed = 10f;
        public float friction;

        [Header("Acceleration")]
        public bool useAcceleration;

        [Header("Ground Acceleration")]
        [Range(0f, 100f)] public float maxAcceleration = 50f;
        [Range(0f, 100f)] public float maxDeceleration = 50f;
        [Range(0f, 100f)] public float maxTurnSpeed = 80f;

        [Header("Air Acceleration")]
        [Range(0f, 100f)] public float maxAirAcceleration = 50f;
        [Range(0f, 100f)] public float maxAirDeceleration = 50f;
        [Range(0f, 100f)] public float maxAirTurnSpeed = 80f;
    }
}
