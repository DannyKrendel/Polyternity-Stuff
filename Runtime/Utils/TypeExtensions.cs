using System;
using System.Reflection;

namespace PolyternityStuff.Utils
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Find field in type and its base types.
        /// </summary>
        public static FieldInfo GetFieldRecursive(this Type type, string name, BindingFlags bindingAttr)
        {
            if (string.IsNullOrEmpty(name)) 
                return null;
            
            FieldInfo fieldInfo = null;
            var parentType = type;
            
            while (parentType != null)
            {
                fieldInfo = parentType.GetField(name, bindingAttr);
                
                if (fieldInfo != null)
                    break;
                
                parentType = parentType.BaseType;
            }

            return fieldInfo;
        }

        /// <summary>
        /// Find member in type and its base types.
        /// </summary>
        public static MemberInfo[] GetMemberRecursive(this Type type, string name, BindingFlags bindingAttr)
        {
            if (string.IsNullOrEmpty(name)) 
                return null;
            
            MemberInfo[] memberInfos = null;
            var parentType = type;
            
            while (parentType != null)
            {
                memberInfos = parentType.GetMember(name, bindingAttr);
                
                if (memberInfos.Length > 0)
                    break;
                
                parentType = parentType.BaseType;
            }

            return memberInfos;
        }
    }
}