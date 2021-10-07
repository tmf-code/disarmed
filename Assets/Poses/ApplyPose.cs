using System.Linq;
using UnityEngine;

public class ApplyPose : MonoBehaviour
{
  public bool apply = false;
  [Range(0, 1)]
  public float strength = 0.0F;

  private PoseSelector poseSelector;
  private Transform model;

  public void Start()
  {
    poseSelector = gameObject.GetComponentOrThrow<PoseSelector>();
    model = transform.FindRecursiveOrThrow("Model");

  }

  public void Update()
  {
    if (!apply) return;
    void ApplyTransform(Transform current)
    {
      if (!BoneNameToBoneId.IsTrackedBone(current.name)) return;
      if (!poseSelector.poses.First().transforms.TryGetValue(current.name, out var target)) return;

      current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);

    }
    ApplyTransform(model);
    model.TraverseChildren(ApplyTransform);
  }
}