using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PolyternityStuff.Editor.Utils
{
    public static class AssetUtils
    {
        /// <summary>
        /// Finds and loads the first asset by filter in given folders.
        /// </summary>
        public static T FindAndLoadAsset<T>(string filter, string[] searchInFolders) where T : Object
        {
            var foundAssets = AssetDatabase.FindAssets(filter, searchInFolders);

            if (foundAssets == null || foundAssets.Length == 0)
            {
                Debug.LogError("Asset was not found");
                return null;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
        
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        
        /// <summary>
        /// Finds and loads ScriptableObject by type.
        /// </summary>
        public static T GetScriptableObject<T>(out string path) where T : ScriptableObject
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}").Select(AssetDatabase.GUIDToAssetPath);
            path = paths.FirstOrDefault();
            
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        /// <summary>
        /// Finds and loads ScriptableObject by type and name.
        /// </summary>
        public static T GetScriptableObject<T>(string name, out string path) where T : ScriptableObject
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}").Select(AssetDatabase.GUIDToAssetPath);
            path = paths.FirstOrDefault(p => p.EndsWith(name + ".asset"));
            
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
