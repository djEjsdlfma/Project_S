using System.Reflection;
using MoonLib.ScriptFinder_Pro.RunTime.Attribute;
using UnityEditor;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.Editor.Attribute
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute info = (InfoBoxAttribute)attribute;

            bool show = true;
            if (!string.IsNullOrEmpty(info.VisibleIf))
            {
                object target = property.serializedObject.targetObject;
                System.Type type = target.GetType();
                var fi = type.GetField(info.VisibleIf,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (fi != null)
                    show = (bool)fi.GetValue(fi.IsStatic ? null : target);
                else
                {
                    var pi = type.GetProperty(info.VisibleIf,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (pi != null)
                        show = (bool)pi.GetValue(pi.GetMethod.IsStatic ? null : target, null);
                }
            }

            float helpHeight = 0;
            if (show)
            {
                helpHeight =
                    EditorStyles.helpBox.CalcHeight(new GUIContent(info.Message), EditorGUIUtility.currentViewWidth)
                    + EditorGUIUtility.standardVerticalSpacing;
            }

            float propHeight;
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                propHeight = EditorGUIUtility.singleLineHeight;
                if (property.isExpanded)
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        var elem = property.GetArrayElementAtIndex(i);
                        propHeight += EditorGUI.GetPropertyHeight(elem, true) +
                                      EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            else
            {
                propHeight = EditorGUI.GetPropertyHeight(property, label, true);
            }

            return helpHeight + propHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute info = (InfoBoxAttribute)attribute;

            bool show = true;
            if (!string.IsNullOrEmpty(info.VisibleIf))
            {
                object target = property.serializedObject.targetObject;
                System.Type type = target.GetType();
                var fi = type.GetField(info.VisibleIf,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (fi != null)
                    show = (bool)fi.GetValue(fi.IsStatic ? null : target);
                else
                {
                    var pi = type.GetProperty(info.VisibleIf,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (pi != null)
                        show = (bool)pi.GetValue(pi.GetMethod.IsStatic ? null : target, null);
                }
            }
            
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                Rect headerRect = position;
                headerRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(headerRect, property, label, false);

                float y = headerRect.y + headerRect.height;
                if (show)
                {
                    float helpHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(info.Message), position.width);
                    Rect helpRect = new Rect(position.x, y, position.width, helpHeight);
                    EditorGUI.HelpBox(helpRect, info.Message, InfoTypeToMessageType(info.Type));
                    y += helpHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                if (property.isExpanded)
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        SerializedProperty element = property.GetArrayElementAtIndex(i);
                        float elemHeight = EditorGUI.GetPropertyHeight(element, true);
                        Rect elemRect = new Rect(position.x, y, position.width, elemHeight);
                        EditorGUI.PropertyField(elemRect, element, true);
                        y += elemHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            else
            {
                float y = position.y;
                if (show)
                {
                    float helpHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(info.Message), position.width);
                    Rect helpRect = new Rect(position.x, y, position.width, helpHeight);
                    EditorGUI.HelpBox(helpRect, info.Message, InfoTypeToMessageType(info.Type));
                    y += helpHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                Rect fieldRect = new Rect(position.x, y, position.width,
                    EditorGUI.GetPropertyHeight(property, label, true));
                EditorGUI.PropertyField(fieldRect, property, label, true);
            }
        }

        public MessageType InfoTypeToMessageType(InfoType infoType)
        {
            switch (infoType)
            {
                case InfoType.Info:
                    return MessageType.Info;
                case InfoType.Warning:
                    return MessageType.Warning;
                case InfoType.Error:
                    return MessageType.Error;
                default:
                    return MessageType.Info;
            }
        }
    }
}