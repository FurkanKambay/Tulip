using System;
using FK.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace FK.Tulip.Weather
{
    public class RainDetector : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BoxCollider2D rainCollider;
        [SerializeField] private LayerMask rainBlockingLayers;
        [SerializeField, Min(0)] private float maxDistance;

        [Header("State")]
        [SerializeField, Range(0, 180)] private float angle;
        [SerializeField] private RainExposureLevel rainExposureLevel;
        [SerializeField, Min(0)] private float rainExposureTimer;

        private Vector2 rainDirection;
        private int exposedCornerCount;

        private readonly Vector2[] corners = new Vector2[CORNERS];
        private readonly RaycastHit2D[] hits = new RaycastHit2D[CORNERS];
        private const int CORNERS = 4;

        private void FixedUpdate()
        {
            rainDirection = angle.ToDirection();
            CastCornerRays(rainCollider);

            rainExposureLevel = exposedCornerCount switch
            {
                1 => RainExposureLevel.Light,
                2 => RainExposureLevel.Moderate,
                3 => RainExposureLevel.Moderate, // angles are sometimes too close
                4 => RainExposureLevel.Maximum,
                _ => RainExposureLevel.None
            };

            if (rainExposureLevel is RainExposureLevel.None)
                rainExposureTimer = 0;
            else
                rainExposureTimer += Time.deltaTime;
        }

        private void CastCornerRays(BoxCollider2D collider)
        {
            Bounds bounds = collider.bounds;
            Vector2 center = bounds.center;
            Vector3 size = bounds.size * 0.5f;

            corners[0] = center + new Vector2(-size.x, -size.y);
            corners[1] = center + new Vector2(size.x, -size.y);
            corners[2] = center + new Vector2(-size.x, size.y);
            corners[3] = center + new Vector2(size.x, size.y);

            int exposed = hits.Length;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(corners[i], rainDirection, maxDistance, rainBlockingLayers);
                hits[i] = hit;

                if (hit)
                    exposed--;
            }

            exposedCornerCount = exposed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            string text = $"Rain: {rainExposureLevel}";
            Vector3 labelPosition = transform.position + (Vector3.up * 2);
            Handles.Label(labelPosition, text, new GUIStyle(GUI.skin.box) { fontSize = 16 });

            for (int index = 0; index < hits.Length; index++)
            {
                Vector2 cornerPoint = corners[index];
                RaycastHit2D hit = hits[index];

                float length = hit ? hit.distance : maxDistance;
                Gizmos.color = hit ? Color.green : Color.red;
                Gizmos.DrawRay(cornerPoint, rainDirection * length);

                if (hit)
                    Gizmos.DrawSphere(hit.point, 0.1f);
            }
        }
#endif
    }
}
