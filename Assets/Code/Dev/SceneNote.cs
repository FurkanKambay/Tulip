using UnityEngine;
using Vertx.Debugging;

namespace FK.Tulip.Dev
{
    [ExecuteAlways]
    internal sealed class SceneNote : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset;

        [Header("Text")]
        [SerializeField, Multiline(lines: 10)] private string text;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color bgColor = Color.black;

        private Camera camera;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position + offset;
            D.raw(new Shape.Text(position, text, camera), bgColor, textColor);
        }
    }
}
