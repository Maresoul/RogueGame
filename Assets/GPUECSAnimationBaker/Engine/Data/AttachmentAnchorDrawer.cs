#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(AttachmentAnchor))]
    public class AttachmentAnchorDrawer : PropertyDrawer
    {
        private Rect GetLineRect(Rect position, int line, float indent)
        {
            return new Rect(position.x + indent, position.y + line * EditorGUIUtility.singleLineHeight,
                position.width - indent, EditorGUIUtility.singleLineHeight);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty attachmentAnchorIDProperty = property.FindPropertyRelative("attachmentAnchorID");
            SerializedProperty attachmentAnchorTransformProperty = property.FindPropertyRelative("attachmentAnchorTransform");
            int line = 0;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), attachmentAnchorIDProperty, new GUIContent("Anchor ID"));
            line++;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), attachmentAnchorTransformProperty, new GUIContent("Anchor Transform reference"));
            EditorGUI.EndProperty();
        }
    }
}
#endif