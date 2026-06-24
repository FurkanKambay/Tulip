using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Furkan.Common
{
    public interface IGameEvent
    {
        event Action OnRaised;
        void Raise(Object sender);
    }

    public interface IGameEvent<T>
    {
        event Action<T> OnRaised;
        void Raise(Object sender, T arg);
    }

    [CreateAssetMenu(menuName = "Game Events/Basic")]
    public class GameEvent : ScriptableObject, IGameEvent
    {
        public event Action OnRaised;

#if UNITY_EDITOR
        [SerializeField] private bool logInvocations;
        [SerializeField, Multiline] private string documentation;
#endif

        public void Raise(Object sender)
        {
            if (logInvocations)
                LogInfo(this, sender);

            OnRaised?.Invoke();
        }

        [HideInCallstack]
        protected internal static void LogInfo(ScriptableObject context, Object sender) =>
            Log.Info($"<color=#777>Event <color=#aae>{context.name}</color> from <color=white>{sender.name}", sender);
    }

    public abstract class GameEvent<T> : ScriptableObject, IGameEvent<T>
    {
        public event Action<T> OnRaised;

        [SerializeField] private bool logInvocations;
#if UNITY_EDITOR
        [SerializeField, Multiline] private string documentation;
#endif

        public void Raise(Object sender, T args)
        {
            if (logInvocations)
                GameEvent.LogInfo(this, sender);

            OnRaised?.Invoke(args);
        }
    }
}
