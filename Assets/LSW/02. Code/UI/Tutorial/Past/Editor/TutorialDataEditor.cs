using LSW._02._Code.Core.Cores;
using UnityEditor;
using UnityEngine;

namespace LSW._02._Code.UI.Tutorial.Editor
{
    [CustomPropertyDrawer(typeof(TutorialData))]
    public class TutorialDataEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty typeProp = property.FindPropertyRelative("tutorialType");
            SerializedProperty endEventProp = property.FindPropertyRelative("onTutorialEnd");
            SerializedProperty textEventProp = property.FindPropertyRelative("tutorialText");
            SerializedProperty imageEventProp = property.FindPropertyRelative("tutorialImage");

            float currentY = position.y;

            Rect typeRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(typeRect, typeProp);
            currentY += EditorGUIUtility.singleLineHeight + 2f;
            
            if (typeProp.intValue == (int)TutorialType.NormalText || typeProp.intValue == (int)TutorialType.Dialogue)
            {
                if (textEventProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(textEventProp);
                    Rect textRect = new Rect(position.x, currentY, position.width, height);
                    EditorGUI.PropertyField(textRect, textEventProp, true);
                    currentY += height + 2f;
                }
            }
            else if (typeProp.intValue == (int)TutorialType.Image)
            {
                if (imageEventProp != null)
                {
                    float height = EditorGUI.GetPropertyHeight(imageEventProp);
                    Rect imageRect = new Rect(position.x, currentY, position.width, height);
                    EditorGUI.PropertyField(imageRect, imageEventProp, true);
                    currentY += height + 2f;
                }
            }
            
            float eventHeight = EditorGUI.GetPropertyHeight(endEventProp);
            Rect eventRect = new Rect(position.x, currentY, position.width, eventHeight);
            EditorGUI.PropertyField(eventRect, endEventProp, true);

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight + 2f;

            SerializedProperty typeProp = property.FindPropertyRelative("tutorialType");
            SerializedProperty endEventProp = property.FindPropertyRelative("onTutorialEnd");
            SerializedProperty textEventProp = property.FindPropertyRelative("tutorialText");
            SerializedProperty imageEventProp = property.FindPropertyRelative("tutorialImage");
            
            if (typeProp.intValue == (int)TutorialType.NormalText || typeProp.intValue == (int)TutorialType.Dialogue)
            {
                if (textEventProp != null) 
                    totalHeight += EditorGUI.GetPropertyHeight(textEventProp) + 2f;
            }
            else if (typeProp.intValue == (int)TutorialType.Image)
            {
                if (imageEventProp != null) 
                    totalHeight += EditorGUI.GetPropertyHeight(imageEventProp) + 2f;
            }
            
            totalHeight += EditorGUI.GetPropertyHeight(endEventProp);

            return totalHeight;
        }
    }
}