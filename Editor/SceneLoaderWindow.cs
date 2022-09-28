using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Polyternity.Editor;
using Polyternity.Editor.Utils;
using PolyternityStuff.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PolyternityStuff.Editor
{
    public class SceneLoaderWindow : EditorWindow
    {
        private const string EditorPrefsScenesKey = "SceneLoaderWindow_Scenes";
        private static SceneGroupCollection _sceneGroupCollection;
        private static string _sceneGroupCollectionPath;

        private static string[] _scenePathsToLoad;
        
        private void OnEnable()
        {
            _sceneGroupCollection = AssetUtils.GetScriptableObject<SceneGroupCollection>(out _sceneGroupCollectionPath);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        [MenuItem("Window/Scene Loader")]
        private static void Init() => GetWindow<SceneLoaderWindow>("Scene Loader");

        private void OnGUI()
        {
            if (_sceneGroupCollection == null)
            {
                GUILayout.Label("Create SceneGroupCollection anywhere in the project.");
                return;
            }
            
            GUILayout.Label($"Loaded from {_sceneGroupCollectionPath}");
            
            foreach (var sceneGroup in _sceneGroupCollection)
            {
                if (GUILayout.Button(sceneGroup.Name) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    _scenePathsToLoad = sceneGroup.Scenes.Select(s => s.ScenePath).ToArray();
                    EditorApplication.EnterPlaymode();
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                OnEnteredEditMode();
            }
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SaveCurrentScenes();
                OnExitingEditMode();
            }
        }

        private static void OnEnteredEditMode()
        {
            _scenePathsToLoad = null;
            var scenes = EditorPrefs.GetString(EditorPrefsScenesKey).Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            EditorPrefs.DeleteKey(EditorPrefsScenesKey);
        
            int i = 0;
            foreach (var scene in scenes)
            {
                EditorSceneManager.OpenScene(scene, i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
                i++;
            }
        }

        private static void OnExitingEditMode()
        {
            for (var i = 0; i < _scenePathsToLoad.Length; i++)
            {
                EditorSceneManager.OpenScene(_scenePathsToLoad[i], i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }
        }
        
        private static void SaveCurrentScenes()
        {
            var scenes = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes += SceneManager.GetSceneAt(i).path + ",";
            }
            EditorPrefs.SetString(EditorPrefsScenesKey, scenes);
        }
    }
}