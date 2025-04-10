using System;
using Furkan.Common;
using SaintsField;
using Tulip.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Transform subject;

        [Header("Gameplay Config")]
        [SerializeField] TrackingOptions trackingConfig;
        [SerializeField] ZoomOptions zoomConfig;

        private new Camera camera;
        private IPlayerBrain brain;

#region Unity Callbacks

        private void Awake()
        {
            camera = Camera.main;
            brain = subject.GetComponentInChildren<IPlayerBrain>();

            Assert.IsNotNull(camera);
            Assert.IsNotNull(brain);

            camera.transform.position = subject.position;
        }

        private void Update()
        {
            trackingConfig.Target = subject.position + (Vector3)trackingConfig.Offset;
            zoomConfig.Target -= brain.ZoomDelta * zoomConfig.Sensitivity * Time.deltaTime;
        }

        private void LateUpdate()
        {
            camera.orthographicSize = Mathf.Lerp(
                camera.orthographicSize,
                zoomConfig.Target,
                Time.deltaTime * zoomConfig.Speed
            );

            Vector3 position = camera.transform.position;
            float lerpX = Mathf.Lerp(position.x, trackingConfig.Target.x, Time.deltaTime * trackingConfig.Speed.x);
            float lerpY = Mathf.Lerp(position.y, trackingConfig.Target.y, Time.deltaTime * trackingConfig.Speed.y);

            Vector3 distance = trackingConfig.Target - position;
            float targetX = Mathf.Abs(distance.x) < trackingConfig.SnapValue ? trackingConfig.Target.x : lerpX;
            float targetY = Mathf.Abs(distance.y) < trackingConfig.SnapValue ? trackingConfig.Target.y : lerpY;
            camera.transform.position = new Vector3(targetX, targetY, trackingConfig.Target.z);
        }
#endregion

#region Subclasses

        [Serializable]
        public class TrackingOptions : IValidate
        {
            private Vector3 target;

            public Vector3 Target
            {
                get => target;
                set => target = value.With(z: -10f);
            }

            [field: SerializeField] public Vector2 Offset { get; private set; }
            [field: SerializeField] public Vector2 Speed { get; private set; } = Vector2.one * 10f;
            [field: SerializeField] public float SnapValue { get; private set; } = .5f;

            public void OnValidate() { }
        }

        [Serializable]
        public class ZoomOptions : IValidate
        {
            [SerializeField] float target = 10f;

            public float Target
            {
                get => target;
                set => this.target = Mathf.Clamp(value, Min, Max);
            }

            [field: SerializeField] public float Min { get; private set; } = 1f;
            [field: SerializeField] public float Max { get; private set; } = 100f;
            [field: SerializeField] public float Sensitivity { get; private set; } = .01f;
            [field: SerializeField] public float Speed { get; private set; } = 10f;

            public void OnValidate() => Target = target;
        }

#endregion
    }
}
