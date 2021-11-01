using UnityEngine;

public class CopyHandTrackingFrom : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  public GameObject trackingDataTarget;
  private Transform model;

  void Start()
  {
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    void CopyTransform(Transform target)
    {
      if (!BoneNameToBoneId.IsTrackedBone(target.name)) return;
      if (target.name == "b_l_forearm_stub" || target.name == "b_r_forearm_stub")
      {
        // Bone shouldn't be controlled by tracking. Conflicts with IK
        return;
      }

      var current = model.FindChildRecursive(target.name);
      if (current == null) return;
      current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);
    }

    trackingDataTarget.GetComponentOrThrow<VRTrackingHierarchy>().vrTrackingData.TraverseChildren(CopyTransform);
  }
}
