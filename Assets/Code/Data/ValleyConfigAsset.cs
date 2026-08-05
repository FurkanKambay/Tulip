using UnityEngine;

namespace FK.Tulip.Data
{
    public interface IValleyConfig
    {
        float RainAngle { get; }
    }

    [CreateAssetMenu(menuName = "World/Valley Config")]
    public class ValleyConfigAsset : ScriptableObject, IValleyConfig
    {
        [SerializeField, Range(-90, 90)] private float rainAngle = 15f;

        public float RainAngle => rainAngle;
    }
}
