using UnityEngine;

namespace FK.Common
{
    // ReSharper disable once IdentifierTypo
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() =>
            DontDestroyOnLoad(gameObject);
    }
}
