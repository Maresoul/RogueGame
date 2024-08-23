#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(GpuEcsAnimationBakerData))]
    public class GpuEcsAnimationBakerDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty animationsProperty = property.FindPropertyRelative("animations");
            SerializedProperty generateAnimationIdsEnumProperty = property.FindPropertyRelative("generateAnimationIdsEnum");
            SerializedProperty animationIdsEnumNameProperty = property.FindPropertyRelative("animationIdsEnumName");

            SerializedProperty usePredefinedAnimationEventIdsProperty = property.FindPropertyRelative("usePredefinedAnimationEventIds");
            SerializedProperty predefinedAnimationEventIdsProperty = property.FindPropertyRelative("predefinedAnimationEventIds");
            SerializedProperty generateAnimationEventIdsEnumProperty = property.FindPropertyRelative("generateAnimationEventIdsEnum");
            SerializedProperty animationEventIdsEnumNameProperty = property.FindPropertyRelative("animationEventIdsEnumName");
            
            SerializedProperty attachmentAnchorsProperty = property.FindPropertyRelative("attachmentAnchors");
            SerializedProperty generateAttachmentAnchorIdsEnumProperty = property.FindPropertyRelative("generateAttachmentAnchorIdsEnum");
            SerializedProperty attachmentAnchorIdsEnumNameProperty = property.FindPropertyRelative("attachmentAnchorIdsEnumName");
            
            SerializedProperty boneUsageProperty = property.FindPropertyRelative("boneUsage");
            
            SerializedProperty transformUsageFlagsParentProperty = property.FindPropertyRelative("transformUsageFlagsParent");
            SerializedProperty transformUsageFlagsChildrenProperty = property.FindPropertyRelative("transformUsageFlagsChildren");

            EditorGUILayout.PropertyField(animationsProperty);
            EditorGUILayout.PropertyField(generateAnimationIdsEnumProperty);
            if(generateAnimationIdsEnumProperty.boolValue)
                EditorGUILayout.PropertyField(animationIdsEnumNameProperty);
            
            EditorGUILayout.PropertyField(usePredefinedAnimationEventIdsProperty);
            if(usePredefinedAnimationEventIdsProperty.boolValue)
                EditorGUILayout.PropertyField(predefinedAnimationEventIdsProperty);
            EditorGUILayout.PropertyField(generateAnimationEventIdsEnumProperty);
            if(generateAnimationEventIdsEnumProperty.boolValue)
                EditorGUILayout.PropertyField(animationEventIdsEnumNameProperty);
            
            EditorGUILayout.PropertyField(attachmentAnchorsProperty);
            EditorGUILayout.PropertyField(generateAttachmentAnchorIdsEnumProperty);
            if(generateAttachmentAnchorIdsEnumProperty.boolValue)
                EditorGUILayout.PropertyField(attachmentAnchorIdsEnumNameProperty);
            
            EditorGUILayout.PropertyField(boneUsageProperty);
            
            EditorGUILayout.PropertyField(transformUsageFlagsParentProperty);
            EditorGUILayout.PropertyField(transformUsageFlagsChildrenProperty);
        }
    }
}
#endif