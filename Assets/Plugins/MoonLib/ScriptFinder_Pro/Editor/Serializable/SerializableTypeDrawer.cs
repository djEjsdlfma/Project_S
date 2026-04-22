#if UNITY_EDITOR
using System;
using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.Serializable;
using UnityEditor;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.Editor.Serializable
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeObjectFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeNameProp = property.FindPropertyRelative("typeName");

            Type currentType = null;
            if (!string.IsNullOrEmpty(typeNameProp.stringValue))
            {
                currentType = ResolveType(typeNameProp.stringValue);
            }

            EditorGUI.BeginProperty(position, label, property);

            var script = currentType != null ? GetMonoScriptFromType(currentType) : null;
            var newScript = EditorGUI.ObjectField(
                position,
                GUIContent.none,
                script,
                typeof(MonoScript),
                false
            ) as MonoScript;

            if (newScript != script)
            {
                var newClass = newScript != null ? newScript.GetClass() : null;
                if (newClass != null)
                {
                    if (typeof(MonoBehaviour).IsAssignableFrom(newClass) || newClass.IsInterface)
                    {
                        typeNameProp.stringValue = newClass.AssemblyQualifiedName;
                    }
                    else
                    {
                        typeNameProp.stringValue = "";
                    }
                }
                else
                {
                    typeNameProp.stringValue = "";
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private MonoScript GetMonoScriptFromType(Type type)
        {
            if (type == null) return null;
            string typeFullName = type.FullName;
            var guids = AssetDatabase.FindAssets("t:MonoScript");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var ms = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (ms != null)
                {
                    var msClass = ms.GetClass();
                    if (msClass?.FullName == typeFullName)
                        return ms;
                }
            }
            return null;
        }

        private Type ResolveType(string nameOrAssemblyQualified)
        {
            var t = Type.GetType(nameOrAssemblyQualified);
            if (t != null) return t;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(nameOrAssemblyQualified) ?? asm.GetTypes().FirstOrDefault(x => x.FullName == nameOrAssemblyQualified);
                if (t != null) return t;
            }
            return null;
        }
    }
}
#endif
