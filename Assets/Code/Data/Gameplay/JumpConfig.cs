using UnityEngine;

namespace FK.Tulip.Data.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/Character Jump")]
    public class JumpConfig : ScriptableObject
    {
        [Range(0, 10f)] public float jumpHeight = 5f;
        [Range(0, 3f)] public float timeToJumpApex = .5f;
        [Range(0, 100f)] public float maxFallSpeed = 10f;

        [Header("Jump Cutoff")]
        public bool hasVariableJumpHeight;

        [Range(.5f, 20f), Tooltip("Gravity multiplier when you let go of jump")]
        public float jumpCutOff;

        [Header("Feeling")]
        [Range(0, .5f)] public float coyoteTime = 0.15f;
        [Range(0, 1f)] public float jumpBuffer = 0.15f;

        [Header("Gravity Multipliers")]
        [Range(0, 10f), Tooltip("Gravity multiplier to apply when going up")]
        public float upwardGravityMultiplier = 1f;

        [Range(0, 30f), Tooltip("Gravity multiplier to apply when coming down")]
        public float downwardGravityMultiplier = 2f;
    }
}
