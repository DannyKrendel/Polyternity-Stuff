using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PolyternityStuff.SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Group Collection", fileName = "New scene group collection")]
    public class SceneGroupCollection : ScriptableObject, IEnumerable<SceneGroup>
    {
        [SerializeField] private List<SceneGroup> _sceneGroups;

        public SceneGroup Find(string name)
        {
            return _sceneGroups.FirstOrDefault(x =>
                string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    
        public bool IsSceneInGroup(string groupName, Scene scene) =>
            Find(groupName).Scenes.Any(s => s.BuildIndex == scene.buildIndex);

        public IEnumerator<SceneGroup> GetEnumerator() => _sceneGroups.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Serializable]
    public struct SceneGroup
    {
        public string Name;
        public List<SceneReference> Scenes;
    }
}