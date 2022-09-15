using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace Polyternity.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static Type GetManagedReferenceFieldType(this SerializedProperty prop)
        {
            var method = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor")
                .GetMethod("GetFieldInfoAndStaticTypeFromProperty",
                    BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = {prop, null};
            method.Invoke(null, parameters);

            return (Type) parameters[1];
        }

        public static Type GetManagedReferenceFullType(this SerializedProperty prop)
        {
            var typename = prop.managedReferenceFullTypename;

            if (string.IsNullOrEmpty(typename))
                return null;

            var assemblyAndType = prop.managedReferenceFullTypename.Split(' ');
            return Type.GetType($"{assemblyAndType[1]}, {assemblyAndType[0]}");
        }

        public static object GetTargetObject(this SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            if (!(GetValue_Imp(source, name) is IEnumerable enumerable)) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
