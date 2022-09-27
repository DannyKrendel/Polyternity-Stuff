using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Polyternity.Editor
{
    /// <summary>
    /// Finds custom property drawer for a given type.
    /// From https://forum.unity.com/threads/solved-custompropertydrawer-not-being-using-in-editorgui-propertyfield.534968/
    /// </summary>
    internal static class PropertyDrawerFinder
    {
        private struct TypeAndFieldInfo
        {
            internal Type Type;
            internal FieldInfo FieldInfo;
        }

        // Rev 3, be more evil with more cache!
        private static readonly Dictionary<int, TypeAndFieldInfo> PathHashVsType = new();

        private static readonly Dictionary<Type, PropertyDrawer> TypeVsDrawerCache = new();

        /// <summary>
        /// Searches for custom property drawer for given property, or returns null if no custom property drawer was found.
        /// </summary>
        public static PropertyDrawer FindDrawerForProperty(SerializedProperty property)
        {
            var pathHash = _GetUniquePropertyPathHash(property);

            if (!PathHashVsType.TryGetValue(pathHash, out var tfi))
            {
                tfi.Type = _GetPropertyType(property, out tfi.FieldInfo);
                PathHashVsType[pathHash] = tfi;
            }

            if (tfi.Type == null)
                return null;

            if (!TypeVsDrawerCache.TryGetValue(tfi.Type, out var drawer))
            {
                drawer = FindDrawerForType(tfi.Type);
                TypeVsDrawerCache.Add(tfi.Type, drawer);
            }

            if (drawer != null)
            {
                // Drawer created by custom way like this will not have "fieldInfo" field installed
                // It is an optional, but some user code in advanced drawer might use it.
                // To install it, we must use reflection again, the backing field name is "internal FieldInfo m_FieldInfo"
                // See ref file in UnityCsReference (2019) project. Note that name could changed in future update.
                // unitycsreference\Editor\Mono\ScriptAttributeGUI\PropertyDrawer.cs
                var fieldInfoBacking =
                    typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfoBacking != null)
                    fieldInfoBacking.SetValue(drawer, tfi.FieldInfo);
            }

            return drawer;
        }

        /// <summary>
        /// Gets type of a serialized property.
        /// </summary>
        private static Type _GetPropertyType(SerializedProperty property, out FieldInfo fi)
        {
            // To see real property type, must dig into object that hosts it.
            _GetPropertyFieldInfo(property, out var resolvedType, out fi);
            return resolvedType;
        }

        /// <summary>
        /// For caching.
        /// </summary>
        private static int _GetUniquePropertyPathHash(SerializedProperty property)
        {
            var hash = property.serializedObject.targetObject.GetType().GetHashCode();
            hash += property.propertyPath.GetHashCode();
            return hash;
        }

        private static void _GetPropertyFieldInfo(SerializedProperty property, out Type resolvedType, out FieldInfo fi)
        {
            var fullPath = property.propertyPath.Split('.');
            var parentType = property.serializedObject.targetObject.GetType();
            
            fi = parentType.GetFieldRecursive(fullPath[0], 
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            resolvedType = fi.FieldType;

            for (var i = 1; i < fullPath.Length; i++)
            {
                // To properly handle array and list
                // This has deeper rabbit hole, see
                // unitycsreference\Editor\Mono\ScriptAttributeGUI\ScriptAttributeUtility.cs GetFieldInfoFromPropertyPath
                // here we will simplify it for now (could break)

                // If we are at 'Array' section like in `tiles.Array.data[0].tilemodId`
                if (_IsArrayPropertyPath(fullPath, i))
                {
                    if (fi.FieldType.IsArray)
                        resolvedType = fi.FieldType.GetElementType();
                    else if (_IsListType(fi.FieldType, out var underlying))
                        resolvedType = underlying;

                    i++; // skip also the 'data[x]' part
                    // In this case, fi is not updated, FieldInfo stay the same pointing to 'tiles' part
                }
                else
                {
                    fi = resolvedType.GetField(fullPath[i]);
                    resolvedType = fi.FieldType;
                }
            }
        }

        private static bool _IsArrayPropertyPath(string[] fullPath, int i)
        {
            // Also search for array pattern, thanks user https://gist.github.com/kkolyan
            // like `tiles.Array.data[0].tilemodId`
            // This is just a quick check, actual check in Unity uses RegEx
            if (fullPath[i] == "Array" && i + 1 < fullPath.Length && fullPath[i + 1].StartsWith("data"))
                return true;
            return false;
        }

        /// <summary>
        /// Stolen from unitycsreference\Editor\Mono\ScriptAttributeGUI\ScriptAttributeUtility.cs
        /// </summary>
        private static bool _IsListType(Type t, out Type containedType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                containedType = t.GetGenericArguments()[0];
                return true;
            }

            containedType = null;
            return false;
        }

        /// <summary>
        /// Returns custom property drawer for type if one could be found, or null if
        /// no custom property drawer could be found. Does not use cached values, so it's resource intensive.
        /// </summary>
        public static PropertyDrawer FindDrawerForType(Type propertyType)
        {
            var cpdType = typeof(CustomPropertyDrawer);
            var typeField = cpdType.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            var childField = cpdType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

            // Optimization note:
            // For benchmark (on DungeonLooter 0.8.4)
            // - Original, search all assemblies and classes: 250 msec
            // - Wappen optimized, search only specific name assembly and classes: 5 msec

            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Wappen optimization: filter only "*Editor" assembly
                if (!assem.FullName.Contains("Editor"))
                    continue;

                foreach (var candidate in assem.GetTypes())
                {
                    // Wappen optimization: filter only "*Drawer" class name, like "SomeTypeDrawer"
                    if (!candidate.Name.Contains("Drawer"))
                        continue;

                    // See if this is a class that has [CustomPropertyDrawer( typeof( T ) )]
                    foreach (var a in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                        if (a.GetType().IsSubclassOf(typeof(CustomPropertyDrawer)) ||
                            a.GetType() == typeof(CustomPropertyDrawer))
                        {
                            var drawerAttribute = (CustomPropertyDrawer)a;
                            var drawerType = (Type)typeField.GetValue(drawerAttribute);
                            if (drawerType == propertyType ||
                                (bool)childField.GetValue(drawerAttribute) && propertyType.IsSubclassOf(drawerType) ||
                                (bool)childField.GetValue(drawerAttribute) &&
                                IsGenericSubclass(drawerType, propertyType))
                                if (candidate.IsSubclassOf(typeof(PropertyDrawer)))
                                {
                                    // Technical note: PropertyDrawer.fieldInfo will not available via this drawer
                                    // It has to be manually setup by caller.
                                    var drawer = (PropertyDrawer)Activator.CreateInstance(candidate);
                                    return drawer;
                                }
                        }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if the parent type is generic and the child type implements it.
        /// </summary>
        private static bool IsGenericSubclass(Type parent, Type child)
        {
            if (!parent.IsGenericType) return false;

            var currentType = child;
            var isAccessor = false;
            while (!isAccessor && currentType != null)
            {
                if (currentType.IsGenericType &&
                    currentType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition())
                {
                    isAccessor = true;
                    break;
                }

                currentType = currentType.BaseType;
            }

            return isAccessor;
        }
    }
}