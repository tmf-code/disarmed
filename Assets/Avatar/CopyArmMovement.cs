using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  public GameObject targetArm;
  private Transform model;
  private Handedness handedness;

  void Start()
  {
    model = transform.FindRecursiveOrThrow("Model");
    handedness = gameObject.GetComponentOrThrow<Handedness>();

    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    targetArm = handedness.handType switch
    {
      Handedness.HandTypes.HandLeft => playerArms.left,
      _ => playerArms.right,
    };
  }

  void Update()
  {
    var handedness = gameObject.GetComponentOrThrow<Handedness>();
    var targetName = handedness.IsLeft()
      ? "ShoulderLeft"
      : "ShoulderRight";


    var targetModel = targetArm.transform.FindRecursiveOrThrow("Model");

    var handPrefix = handedness.HandPrefix();

    // Copy IK from other arm
    var forearm = model.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub");
    var humerus = model.FindRecursiveOrThrow($"b_{handPrefix}_humerus");

    var forearmOther = targetModel.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub");
    var humerusOther = targetModel.FindRecursiveOrThrow($"b_{handPrefix}_humerus");

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
