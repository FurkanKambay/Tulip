using FK.Common;
using FK.Tulip.Input;
using UnityEngine;

namespace FK.Tulip.Character
{
    public class CharacterFacer : MonoBehaviour
    {
        [Header("Brain")]
        [SerializeField, Required] private CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] private SpriteRenderer sprite;

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
