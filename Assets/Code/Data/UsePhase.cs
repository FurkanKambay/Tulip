using System;
using UnityEngine;
using SaintsField;
using SaintsField.Playa;

namespace Tulip.Data
{
    [Serializable]
    public struct UsePhase
    {
        [LayoutGroup(".", ELayout.Background)]
        [LayoutGroup("./Options", ELayout.TitleOut)]
        public bool isCancelable;
        public bool preventAim;
        public bool shouldHit;

        [LayoutGroup("./Transform", ELayout.TitleOut | ELayout.Horizontal)]
        [PostFieldRichLabel("in")]
        public Vector2 moveDelta;

        [OverlayRichLabel("<color=grey>sec")]
        public float moveDuration;

        [LayoutGroup("./Turn", ELayout.Horizontal)]
        [PostFieldRichLabel("in")]
        [OverlayRichLabel("<color=grey>deg")]
        public float turnDelta;

        [OverlayRichLabel("<color=grey>sec")]
        public float turnDuration;
    }
}
