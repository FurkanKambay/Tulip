using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace FK.Common.Events
{
    public interface IGameEvent
    {
        event Action OnRaised;

        void Raise(Object sender,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLine = 0);
    }

    public interface IGameEvent<T>
    {
        event Action<T> OnRaised;

        void Raise(Object sender, T args,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0);
    }
}
