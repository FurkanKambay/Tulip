using FK.Tulip.Audio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FK.Tulip.Editor
{
    [CustomPropertyDrawer(typeof(FMODEvent))]
    public class FMODEventDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            new PropertyField(property.FindPropertyRelative("reference"));
    }
}
