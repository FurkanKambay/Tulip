using Furkan.Common;
using Tulip.Input;
using UnityEngine;

namespace Tulip.Character
{
    public class CharacterFacer : MonoBehaviour
    {
        [Header("Brain")]
        [SerializeField, Required] CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] SpriteRenderer sprite;

        private void Update()
        {
            if (brain == null) return;
            if (brain.HorizontalMovement != 0)
            {
                sprite.flipX = brain.HorizontalMovement < 0;
                return;
            }

            if (!brain.AimPosition.HasValue)
                return;

            Vector2 targetVector = brain.AimPosition.Value - (Vector2)transform.position;
            sprite.flipX = targetVector.x < 0;
        }
    }
}
