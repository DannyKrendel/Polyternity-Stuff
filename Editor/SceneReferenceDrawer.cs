using System;
using System.Linq;
using Polyternity.Editor.Utils;
using PolyternityStuff.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Polyternity.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private SerializedProperty _sceneAssetProperty;
        private string[] _scenePaths;
        private string[] _displayedScenes;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _sceneAssetProperty = property.FindPropertyRelative("_sceneAsset");
            
            if (_scenePaths == null)
            {
                _scenePaths = SceneUtils.GetScenePaths();
                _displayedScenes = _scenePaths.Select(SceneUtils.GetSceneNameFromPath).ToArray();
            }

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    int i = DrawPopup(position, label.text, GetSelectedIndex(property));

                    if (check.changed)
                    {
                        _sceneAssetProperty.objectReferenceValue = EditorSceneHelper.GetAssetFromPath(_scenePaths[i]);
                    }
                }
                
                EditorGUI.indentLevel = indent;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private int GetSelectedIndex(SerializedProperty property)
        {
            if (_sceneAssetProperty.objectReferenceValue != null && property.GetTargetObject() is SceneReference sceneReference)
                return Array.IndexOf(_scenePaths, sceneReference.ScenePath);
            return 0;
        }

        #region Draw calls

        private int DrawPopup(Rect rect, string labelText, int selectedIndex)
        {
            var style = new GUIStyle(EditorStyles.popup) {fixedHeight = EditorGUIUtility.singleLineHeight};
            return EditorGUI.Popup(rect, labelText, selectedIndex, _displayedScenes, style);
        }

        #endregion
    }
}
