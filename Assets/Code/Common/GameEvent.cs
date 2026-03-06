using System;
using UnityEngine;

namespace Furkan.Common
{
    [CreateAssetMenu(menuName = "Game Events/Basic")]
    public class GameEvent : ScriptableObject
    {
        public event Action OnRaised;

        public void Raise() => OnRaised?.Invoke();
    }
}
