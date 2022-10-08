using System.IO;
using UnityEngine.SceneManagement;

namespace PolyternityStuff.Utils
{
    public static class SceneUtils
    {
        /// <summary>
        /// Finds a scene by path and returns its name if it exists.
        /// </summary>
        public static string GetSceneNameFromPath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Returns true if a scene with the given name is loaded.
        /// </summary>
        public static bool IsSceneLoaded(string name)
        {
            return SceneManager.GetSceneByName(name).isLoaded;
        }

        /// <summary>
        /// Returns true if a scene with the given build index is loaded.
        /// </summary>
        public static bool IsSceneLoaded(int buildIndex)
        {
            return SceneManager.GetSceneByBuildIndex(buildIndex).isLoaded;
        }

        /// <summary>
        /// Finds a scene by name and if it's loaded makes it active.
        /// </summary>
        public static void SetActiveScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded)
                SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// Returns all scene paths from build settings.
        /// </summary>
        public static string[] GetScenePaths()
        {
            var scenes = new string[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }

            return scenes;
        }
    }
}