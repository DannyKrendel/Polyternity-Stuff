﻿using UnityEditor;

namespace PolyternityStuff.Editor
{
    public static class MenuItems
    {
        [MenuItem("(๑•ᴗ•๑)/Set Selected Objects Dirty")]
        private static void SetSelectedObjectsDirty() 
        {
            foreach (var o in Selection.objects) 
            {
                EditorUtility.SetDirty(o);
            }
        }
    }
}