using PolyternityStuff.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Polyternity.Editor
{
    [CustomPropertyDrawer(typeof(SceneGroup))]
    public class SceneGroupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
            
                var nameRect = new Rect(position.x, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
                var scenesRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 
                    position.width, EditorGUIUtility.singleLineHeight);
            
                EditorGUI.PropertyField(nameRect, property.FindPropertyRelative(nameof(SceneGroup.Name)), GUIContent.none);
                EditorGUI.PropertyField(scenesRect, property.FindPropertyRelative(nameof(SceneGroup.Scenes)));
            
                EditorGUI.indentLevel = indent;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(SceneGroup.Name))) +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(SceneGroup.Scenes))) + 
                   EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
