using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipmunk.Modules.StatSystem.Editor
{
    [CustomPropertyDrawer(typeof(StatOverride))]
    public class StatOverrideDrawer : PropertyDrawer
    {
        [SerializeField] private Material statIconMaterial;
        private const float ToggleWidth = 95f;
        private const float Padding = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return (line * 2f) + spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty statProperty = property.FindPropertyRelative("stat");
            SerializedProperty useOverrideProperty = property.FindPropertyRelative("isUseOverride");
            SerializedProperty overrideValueProperty = property.FindPropertyRelative("overrideValue");

            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect firstLineRect = new Rect(position.x, position.y, position.width, line);
            Rect secondLineRect = new Rect(position.x, position.y + line + spacing, position.width, line);

            EditorGUI.BeginProperty(position, label, property);

            Rect contentRect = EditorGUI.PrefixLabel(firstLineRect, label);
            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect statRect = contentRect;
            statRect.width -= line + Padding;

            Rect iconRect = new Rect(statRect.xMax + Padding, contentRect.y, line, line);

            EditorGUI.PropertyField(statRect, statProperty, GUIContent.none);
            DrawIcon(iconRect, statProperty.objectReferenceValue);

            Rect toggleRect = new Rect(secondLineRect.x, secondLineRect.y, ToggleWidth, secondLineRect.height);
            Rect valueRect = new Rect(
                toggleRect.xMax + Padding,
                secondLineRect.y,
                secondLineRect.width - toggleRect.width - Padding,
                secondLineRect.height);

            EditorGUI.PropertyField(toggleRect, useOverrideProperty, new GUIContent("Override"));
            using (new EditorGUI.DisabledScope(!useOverrideProperty.boolValue))
            {
                EditorGUI.PropertyField(valueRect, overrideValueProperty, new GUIContent("Value"));
            }

            EditorGUI.indentLevel = previousIndent;
            EditorGUI.EndProperty();
        }

        private void DrawIcon(Rect iconRect, Object statObject)
        {
            Texture texture = TryGetStatIconTexture(statObject);
            if (texture == null)
            {
                EditorGUI.DrawRect(iconRect, new Color(0f, 0f, 0f, 0.08f));
                EditorGUI.LabelField(iconRect, "-", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            EditorGUI.DrawPreviewTexture(iconRect, texture, statIconMaterial, ScaleMode.ScaleToFit);
        }

        private static Texture TryGetStatIconTexture(Object statObject)
        {
            if (statObject == null)
            {
                return null;
            }

            SerializedObject statSerializedObject = new SerializedObject(statObject);
            SerializedProperty iconProperty = statSerializedObject.FindProperty("icon");
            Sprite iconSprite = iconProperty?.objectReferenceValue as Sprite;
            return iconSprite != null ? iconSprite.texture : null;
        }
    }
}