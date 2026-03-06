using UnityEngine;

namespace Tulip.Data
{
    public interface IValidate : ISerializationCallbackReceiver
    {
        void OnValidate();

        void ISerializationCallbackReceiver.OnBeforeSerialize() => OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
