using UnityEditor;

namespace Polyternity.Editor
{
    public static class MenuItems
    {
        [MenuItem("Tools/Set Selected Objects Dirty")]
        private static void SetSelectedObjectsDirty() 
        {
            foreach (var o in Selection.objects) 
            {
                EditorUtility.SetDirty(o);
            }
        }
    }
}