using FK.Tulip.Combat;
using UnityEngine;

namespace FK.Tulip.Player
{
    [ExecuteAlways]
    public class ShaderGlobals : MonoBehaviour
    {
        [SerializeField] Health player;

        private static readonly int shaderPlayerPosition = Shader.PropertyToID("_Player_Position");

        private Transform entityTransform;

        private void Awake() => entityTransform = player.transform;

        private void Update() =>
            Shader.SetGlobalVector(shaderPlayerPosition, entityTransform.position);
    }
}
