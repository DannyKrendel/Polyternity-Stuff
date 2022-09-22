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

            PropertyDrawerFinder.FindDrawerForProperty(property).OnGUI(position, property, label);
            
            GUI.enabled = previousGUIState;
        }
    }

}