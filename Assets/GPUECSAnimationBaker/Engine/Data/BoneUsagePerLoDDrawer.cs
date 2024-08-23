#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [CustomPropertyDrawer(typeof(BoneUsagePerLoD))]
    public class BoneUsagePerLoDDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty lodIndexProperty = property.FindPropertyRelative("lodIndex");
            SerializedProperty maxNumberOfBonesPerVertexProperty = property.FindPropertyRelative("maxNumberOfBonesPerVertex");
            EditorGUI.LabelField(new Rect(position.position, new Vector2(30, position.height)), "LOD");
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(35, 0), new Vector2(30, position.height)), 
                lodIndexProperty, new GUIContent(""));
            EditorGUI.LabelField(new Rect(position.position + new Vector2(70, 0), new Vector2(80, position.height)), "Nbr. of bones");
            EditorGUI.PropertyField(new Rect(position.position + new Vector2(155, 0), new Vector2(position.width - 160, position.height)), 
                maxNumberOfBonesPerVertexProperty, new GUIContent(""));
            EditorGUI.EndProperty();
        }
    }
}
#endif