using UnityEngine;

namespace FK.Tulip.Character
{
    public class ActorAnimator : CharacterAnimator
    {
        [SerializeField] CharacterMovement movement;
        [SerializeField] CharacterJump jumper;

        private static readonly int animSpeed = Animator.StringToHash("speed");
        private static readonly int animJumping = Animator.StringToHash("jumping");

        private void Update() => animator.SetBool(animJumping, jumper.IsJumping);
        private void FixedUpdate() => animator.SetFloat(animSpeed, Mathf.Abs(movement.Velocity.x));
    }
}
