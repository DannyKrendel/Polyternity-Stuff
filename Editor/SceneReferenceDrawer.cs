using System;
using System.Linq;
using Polyternity.Editor;
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
        private string _selectedScenePath;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _sceneAssetProperty = property.FindPropertyRelative("_sceneAsset");
            
            if (_scenePaths == null)
            {
                _scenePaths = SceneUtils.GetScenePaths();
                _displayedScenes = _scenePaths.Select(SceneUtils.GetSceneNameFromPath).ToArray();
                _selectedScenePath = (property.GetTargetObject() as SceneReference)?.ScenePath;
            }

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    int i = DrawPopup(position, GetSelectedIndex(property));

                    if (check.changed)
                    {
                        _sceneAssetProperty.objectReferenceValue = EditorSceneHelper.GetAssetFromPath(_scenePaths[i]);
                        _selectedScenePath = _scenePaths[i];
                    }
                }
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

        private int DrawPopup(Rect position, int selectedIndex)
        {
            var popupRect = new Rect(position.x + 6, position.y, position.width - 6, 20);

            var style = new GUIStyle(EditorStyles.popup) {fixedHeight = 20};
            
            return EditorGUI.Popup(popupRect, selectedIndex, _displayedScenes, style);
        }

        #endregion
    }
}
