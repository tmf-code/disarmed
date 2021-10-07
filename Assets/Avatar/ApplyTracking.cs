using UnityEngine;

public class ApplyTracking : MonoBehaviour
{
  public bool apply = false;
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
    if (!apply) return;
    void CopyTransform(Transform target)
    {
      if (!BoneNameToBoneId.IsTrackedBone(target.name)) return;

      var current = model.FindChildRecursive(target.name);
      if (current == null) return;
      current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);
    }

    model.localPosition = Vector3.Lerp(model.localPosition, trackingData.localPosition, strength);
    model.localRotation = Quaternion.Slerp(model.localRotation, trackingData.localRotation, strength);
    model.localScale = Vector3.Lerp(model.localScale, trackingData.localScale, strength);
    trackingData.TraverseChildren(CopyTransform);
  }
}
