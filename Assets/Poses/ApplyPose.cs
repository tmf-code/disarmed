using System.Linq;
using UnityEditor;
using UnityEngine;

public class ApplyPose : MonoBehaviour
{

  [HideInInspector]
  public PoseSelector poseSelector;

  public void Start()
  {
    poseSelector = gameObject.GetComponentOrThrow<PoseSelector>();
  }

  public void Apply()
  {
    if (!Application.isPlaying) return;
    void ApplyTransform(Transform transform)
    {
      poseSelector.poses.First().transforms.TryGetValue(transform.name, out var savedTransform);
      if (savedTransform == null) return;

      transform.localPosition = savedTransform.localPosition;
      transform.localRotation = savedTransform.localRotation;
      transform.localScale = savedTransform.localScale;
    }
    ApplyTransform(transform);
    transform.TraverseChildren(ApplyTransform);
  }

}

[CustomEditor(typeof(ApplyPose))]
public class ApplyPoseEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawPropertiesExcluding(serializedObject);
    ApplyPose applyPose = (ApplyPose)target;

    if (GUILayout.Button("Apply Transforms"))
    {
      applyPose.Apply();
    }
  }
}