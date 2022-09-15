using System.IO;
using UnityEngine.SceneManagement;

namespace PolyternityStuff.SceneManagement
{
    public static class SceneUtils
    {
        public static string GetSceneNameFromPath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : Path.GetFileNameWithoutExtension(path);
        }

        public static bool IsSceneLoaded(string name)
        {
            return SceneManager.GetSceneByName(name).isLoaded;
        }

        public static bool IsSceneLoaded(int buildIndex)
        {
            return SceneManager.GetSceneByBuildIndex(buildIndex).isLoaded;
        }

        public static void SetActiveScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded)
                SceneManager.SetActiveScene(scene);
        }

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