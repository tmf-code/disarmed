using UnityEngine;

public class ApplyHandTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 0.0F;
  private Transform trackingData;
  private Transform model;

  void Start()
  {
    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    void CopyTransform(Transform target)
    {
      if (!BoneNameToBoneId.IsTrackedBone(target.name)) return;

      var current = model.FindChildRecursive(target.name);
      if (current == null) return;
      current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);
    }
    trackingData.TraverseChildren(CopyTransform);
  }
}
