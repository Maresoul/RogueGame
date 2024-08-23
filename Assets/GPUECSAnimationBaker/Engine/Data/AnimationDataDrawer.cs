#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationDataDrawer : PropertyDrawer
    {
        private Rect GetLineRect(Rect position, int line, float indent)
        {
            return new Rect(position.x + indent, position.y + line * EditorGUIUtility.singleLineHeight,
                position.width - indent, EditorGUIUtility.singleLineHeight);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty animationIDProperty = property.FindPropertyRelative("animationID");
            SerializedProperty animatorStateNameProperty = property.FindPropertyRelative("animatorStateName");
            SerializedProperty animationTypeProperty = property.FindPropertyRelative("animationType");
            SerializedProperty singleClipDataProperty = property.FindPropertyRelative("singleClipData");
            SerializedProperty dualClipBlendDataProperty = property.FindPropertyRelative("dualClipBlendData");
            SerializedProperty loopDataProperty = property.FindPropertyRelative("loop");
            SerializedProperty additionalParameterValuesProperty = property.FindPropertyRelative("additionalAnimatorParameterValues");
            SerializedProperty additionalAnimatorStatesPerLayerProperty = property.FindPropertyRelative("additionalAnimatorStatesPerLayer");
            int line = 0;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), animationIDProperty, new GUIContent("Animation ID"));
            line++;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), animatorStateNameProperty, new GUIContent("Animator State Name"));
            line++;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), animationTypeProperty, new GUIContent("Animation Type"));
            AnimationTypes animationType = (AnimationTypes)animationTypeProperty.enumValueIndex;
            if (animationType == AnimationTypes.SingleClip)
            {
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                    singleClipDataProperty.FindPropertyRelative("animationClip"), new GUIContent("Animation Clip"));
            }
            else
            {
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                    dualClipBlendDataProperty.FindPropertyRelative("blendParameterName"), new GUIContent("Blend parameter name"));
                line++;
                EditorGUI.LabelField(GetLineRect(position, line, 0), "Clip 1"); 
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 10), 
                    dualClipBlendDataProperty.FindPropertyRelative("clip1").FindPropertyRelative("parameterValue"), new GUIContent("Parameter Value"));
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 10), 
                    dualClipBlendDataProperty.FindPropertyRelative("clip1").FindPropertyRelative("animationClip"), new GUIContent("Animation Clip"));
                line++;
                EditorGUI.LabelField(GetLineRect(position, line, 0), "Clip 2"); 
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 10), 
                    dualClipBlendDataProperty.FindPropertyRelative("clip2").FindPropertyRelative("parameterValue"), new GUIContent("Parameter Value"));
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 10), 
                    dualClipBlendDataProperty.FindPropertyRelative("clip2").FindPropertyRelative("animationClip"), new GUIContent("Animation Clip"));
                line++;
                EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                    dualClipBlendDataProperty.FindPropertyRelative("nbrOfInBetweenSamples"), new GUIContent("Nbr of in between samples"));
            }
            line++;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                loopDataProperty, new GUIContent("Loop"));
            line++;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                additionalParameterValuesProperty, new GUIContent("Additional animator parameter values"));
            line++;
            if (additionalParameterValuesProperty.isExpanded)
                line += additionalParameterValuesProperty.arraySize + 3;
            EditorGUI.PropertyField(GetLineRect(position, line, 0), 
                additionalAnimatorStatesPerLayerProperty, new GUIContent("Additional animator states per layer"));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty animationTypeProperty = property.FindPropertyRelative("animationType");
            SerializedProperty additionalParameterValuesProperty = property.FindPropertyRelative("additionalAnimatorParameterValues");
            SerializedProperty additionalAnimatorStatesPerLayerProperty = property.FindPropertyRelative("additionalAnimatorStatesPerLayer");
            AnimationTypes animationType = (AnimationTypes)animationTypeProperty.enumValueIndex;
            int nbrOfLines = animationType == AnimationTypes.SingleClip ? 7 : 14;
            if (additionalParameterValuesProperty.isExpanded)
                nbrOfLines += additionalParameterValuesProperty.arraySize + 3;
            if (additionalAnimatorStatesPerLayerProperty.isExpanded)
                nbrOfLines += additionalAnimatorStatesPerLayerProperty.arraySize + 3;
            return EditorGUIUtility.singleLineHeight * nbrOfLines;
        }
    }
}
#endif