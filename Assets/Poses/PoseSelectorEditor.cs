using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoseSelector))]
public class PoseSelectorEditor : Editor
{
  SerializedProperty posePath;
  SerializedProperty poses;

  void OnEnable()
  {
    posePath = serializedObject.FindProperty("posePath");
    poses = serializedObject.FindProperty("poses");
  }

  public override void OnInspectorGUI()
  {
    serializedObject.Update();
    EditorGUILayout.PropertyField(posePath);
    EditorGUILayout.PropertyField(poses);
    serializedObject.ApplyModifiedProperties();

    PoseSelector poseSelector = (PoseSelector)target;

    if (GUILayout.Button("Load Poses"))
    {
      poseSelector.Load();
    }

  }
}
