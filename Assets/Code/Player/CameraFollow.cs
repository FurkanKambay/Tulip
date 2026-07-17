using System;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace FK.Tulip.Player
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [Header("Gameplay Config")]
        [SerializeField] private TrackingOptions trackingConfig;

        [Header("Injectable State")]
        [SerializeField, Required] private Transform subject;

        private Camera camera;

#region Unity Callbacks
        private void Awake()
        {
            camera = Camera.main;
            Assert.IsNotNull(camera);
        }

        private void Start()
        {
            if (!subject)
            {
                enabled = false;
                return;
            }

            camera.transform.position = subject.position;
        }

        private void Update() =>
            trackingConfig.Target = subject.position + (Vector3)trackingConfig.Offset;

        private void LateUpdate()
        {
            Vector3 position = camera.transform.position;
            float lerpX = Mathf.Lerp(position.x, trackingConfig.Target.x, Time.deltaTime * trackingConfig.Speed.x);
            float lerpY = Mathf.Lerp(position.y, trackingConfig.Target.y, Time.deltaTime * trackingConfig.Speed.y);

            Vector3 distance = trackingConfig.Target - position;
            float targetX = Mathf.Abs(distance.x) < trackingConfig.SnapValue ? trackingConfig.Target.x : lerpX;
            float targetY = Mathf.Abs(distance.y) < trackingConfig.SnapValue ? trackingConfig.Target.y : lerpY;
            camera.transform.position = new Vector3(targetX, targetY, trackingConfig.Target.z);
        }
#endregion

        public void SetTarget(Transform subject) =>
            this.subject = subject;

#region Child Class
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
#endregion
    }
}
