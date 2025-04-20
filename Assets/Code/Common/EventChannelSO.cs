using System;
using UnityEngine;

namespace Furkan.Common
{
    [CreateAssetMenu(menuName = "Events/Event Channel")]
    public class EventChannelSO : ScriptableObject
    {
        public event Action OnRaised;

        public void Raise() => OnRaised?.Invoke();
    }
}
