using UnityEngine;

namespace FK.Common.Events
{
#if UNITY_EDITOR
    public partial class GameEvent
    {
        [SerializeField, Multiline] private string documentation;
    }

    public abstract partial class GameEvent<T>
    {
        [SerializeField, Multiline] private string documentation;
    }
#endif
}
