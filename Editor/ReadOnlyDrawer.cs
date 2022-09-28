using Polyternity.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Polyternity.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIState = GUI.enabled;
            
            GUI.enabled = false;

            var customDrawer = PropertyDrawerFinder.FindDrawerForProperty(property);
            
            if (customDrawer == null)
                EditorGUI.PropertyField(position, property, label);
            else
                customDrawer.OnGUI(position, property, label);

            GUI.enabled = previousGUIState;
        }
    }

}