using UnityEngine;
using Object = UnityEngine.Object;

namespace Tulip.Data.Sets
{
    public abstract class DataSet<T> : ScriptableObject where T : Object
    {
        [SerializeField] protected T[] list;

        public T[] List => list;
        public int Count => list.Length;

        public T this[int index] => list[index];

#if UNITY_EDITOR
        protected virtual void PopulateAllAssets()
        {
            list = Resources.LoadAll<T>(string.Empty);
        }
#endif
    }
}
