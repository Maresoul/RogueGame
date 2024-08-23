#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(AnimatorState))]
    public class AnimatorStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty parameterStateNameProperty = property.FindPropertyRelative("stateName");
            SerializedProperty parameterLayerProperty = property.FindPropertyRelative("layer");
            EditorGUI.LabelField(new Rect(position.position, 
                    new Vector2(80, position.height)),
                "Layer"); 
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(85f, 0f), 
                    new Vector2(80, position.height)), 
                parameterLayerProperty, new GUIContent(""));
            EditorGUI.LabelField(new Rect(position.position + new Vector2(170, 0f), 
                    new Vector2(80, position.height)),
                "State Name"); 
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(255f, 0f), 
                    new Vector2(position.width - 255, position.height)), 
                parameterStateNameProperty, new GUIContent(""));
            EditorGUI.EndProperty();
        }
    }
}
#endif