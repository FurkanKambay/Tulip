using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FK.Common.Events
{
    [CreateAssetMenu(menuName = "Game Events/Basic")]
    public partial class GameEvent : ScriptableObject, IGameEvent
    {
        public event Action OnRaised;

        [SerializeField] private bool logInvocations;

        public void Raise(
            Object sender,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLine = 0
        )
        {
            if (logInvocations)
                LogEvent(this, sender, callerMember, callerFilePath, callerLine);

            OnRaised?.Invoke();
        }

        [HideInCallstack]
        protected internal static void LogEvent(
            ScriptableObject @event,
            Object sender,
            string callerMember,
            string callerFile,
            int callerLine
        )
        {
            string eventName = $"Event <color=#aae>{@event.name}</color>";
            string senderName = $"called via <color=white>{sender.name}</color> from <color=#c191ff>{callerMember}";
            string fileInfo = $"<color=white>{callerFile}:{callerLine}";

            Log.Info($"<color=#aaa>{eventName} {senderName}\n{fileInfo}\n", sender);
        }
    }

    public abstract partial class GameEvent<T> : ScriptableObject, IGameEvent<T>
    {
        public event Action<T> OnRaised;

        [SerializeField] private bool logInvocations;

        public void Raise(
            Object sender,
            T args,
            [CallerMemberName] string callerMember = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0
        )
        {
            if (logInvocations)
                GameEvent.LogEvent(this, sender, callerMember, callerFile, callerLine);

            OnRaised?.Invoke(args);
        }
    }
}
