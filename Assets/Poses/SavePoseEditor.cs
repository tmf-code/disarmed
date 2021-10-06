using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SavePose))]
public class SavePoseEditor : Editor
{
  SerializedProperty filename;
  SerializedProperty start;

  void OnEnable()
  {
    filename = serializedObject.FindProperty("filename");
    start = serializedObject.FindProperty("start");
  }


  public override void OnInspectorGUI()
  {
    serializedObject.Update();
    EditorGUILayout.PropertyField(filename);
    EditorGUILayout.PropertyField(start);

    serializedObject.ApplyModifiedProperties();

    SavePose saveTransforms = (SavePose)target;

    if (GUILayout.Button("Save Pose"))
    {
      saveTransforms.Save();
    }
  }
}
