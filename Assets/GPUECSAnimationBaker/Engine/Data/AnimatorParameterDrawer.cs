#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(AnimatorParameter))]
    public class AnimatorParameterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float w = (position.width - 90) / 2;
            SerializedProperty parameterNameProperty = property.FindPropertyRelative("parameterName");
            SerializedProperty parameterTypeProperty = property.FindPropertyRelative("parameterType");
            SerializedProperty boolValueProperty = property.FindPropertyRelative("boolValue");
            SerializedProperty floatValueProperty = property.FindPropertyRelative("floatValue");
            SerializedProperty intValueProperty = property.FindPropertyRelative("intValue");
            EditorGUI.PropertyField(new Rect(position.position, new Vector2(w, position.height)), 
                parameterNameProperty, new GUIContent(""));
            EditorGUI.BeginChangeCheck();            
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(w + 5, 0), new Vector2(80, position.height)), 
                parameterTypeProperty, new GUIContent(""));
            if (EditorGUI.EndChangeCheck())
            {
                boolValueProperty.boolValue = false;
                floatValueProperty.floatValue = 0;
                intValueProperty.intValue = 0;
            }
            AnimatorParameterTypes parameterType = (AnimatorParameterTypes)parameterTypeProperty.enumValueIndex;
            SerializedProperty valueProperty = null;
            if (parameterType == AnimatorParameterTypes.Bool) valueProperty = boolValueProperty;
            else if (parameterType == AnimatorParameterTypes.Float) valueProperty = floatValueProperty;
            else if (parameterType == AnimatorParameterTypes.Integer) valueProperty = intValueProperty;
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(w + 90, 0), new Vector2(w, position.height)), 
                valueProperty, new GUIContent(""));
            EditorGUI.EndProperty();
        }
    }
}
#endif