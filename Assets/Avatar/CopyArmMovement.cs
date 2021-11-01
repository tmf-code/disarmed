using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  public GameObject targetArm;
  private Transform model;

  void Start()
  {
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    var handedness = gameObject.GetComponentOrThrow<Handedness>();
    var targetName = handedness.IsLeft()
      ? "ShoulderLeft"
      : "ShoulderRight";

    var shoulderAttachmentPoint = GameObject.Find(targetName).transform;
    var targetModel = targetArm.transform.FindOrThrow("Model");
    var rotation = targetModel.rotation * Quaternion.Inverse(shoulderAttachmentPoint.rotation);

    var handPrefix = handedness.HandPrefix();
    var shoulder = model.FindRecursiveOrThrow($"b_{handPrefix}_shoulder");
    var distance = model.transform.position - shoulder.transform.position;

    model.localPosition = Vector3.Lerp(model.localPosition, distance, 1);
    model.localRotation = Quaternion.Slerp(model.localRotation, rotation, strength);

    // Copy IK from other arm
    var wrist = model.FindRecursiveOrThrow($"b_{handPrefix}_wrist");
    var forearm = model.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub");
    var humerus = model.FindRecursiveOrThrow($"b_{handPrefix}_humerus");

    var wristOther = targetModel.FindRecursiveOrThrow($"b_{handPrefix}_wrist");
    var forearmOther = targetModel.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub");
    var humerusOther = targetModel.FindRecursiveOrThrow($"b_{handPrefix}_humerus");

    wrist.localRotation = wristOther.localRotation;
    forearm.localRotation = forearmOther.localRotation;
    humerus.localRotation = humerusOther.localRotation;

    // Apply hand tracking
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

    targetModel.TraverseChildren(CopyTransform);
  }
}
