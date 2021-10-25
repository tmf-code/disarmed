using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
  {
    var valueStr = prop.propertyType switch
    {
      SerializedPropertyType.Integer => prop.intValue.ToString(),
      SerializedPropertyType.Boolean => prop.boolValue.ToString(),
      SerializedPropertyType.Float => prop.floatValue.ToString("0.00000"),
      SerializedPropertyType.String => prop.stringValue,
      _ => "(not supported)",
    };
    EditorGUI.LabelField(position, label.text, valueStr);
  }
}