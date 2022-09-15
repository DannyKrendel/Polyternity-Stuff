using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Polyternity.Editor
{
    public static class AssetUtils
    {
        public static T GetScriptableObject<T>(out string path, string name = null) where T : ScriptableObject
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}").Select(AssetDatabase.GUIDToAssetPath);
            path = name == null ? paths.FirstOrDefault() : paths.FirstOrDefault(p => p.EndsWith(name + ".asset"));
            
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static string GetAssetPathFromTypeAndName(string type, string name)
        {
            var assets = AssetDatabase.FindAssets($"t:{type} {name}");

            if (assets.Length == 0)
            {
                Debug.LogWarning($"Asset {name} of type {type} was not found.");
                return string.Empty;
            }
            if (assets.Length > 1)
            {
                Debug.LogWarning($"Multiple assets with name {name} of type {type} were found.");
                return string.Empty;
            }
            
            return AssetDatabase.GUIDToAssetPath(assets[0]);
        }
    }
}
