using Furkan.Common;
using UnityEditor;
using UnityEngine;

namespace Tulip.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            string label = targets.Length == 1 ? "Raise" : $"Raise ({targets.Length} events)";
            if (!GUILayout.Button(label, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f)))
                return;

            foreach (Object t in targets)
                ((GameEvent)t).Raise(t);
        }
    }
}
