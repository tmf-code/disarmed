using UnityEngine;

public class ApplyInverseKinematics : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private Transform forearmIK;
  private Transform humerusIK;
  private Transform shoulderIK;
  private Transform forearm;
  private Transform humerus;
  private Transform shoulder;

  void Start()
  {
    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var hand = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    forearmIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerusIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;
    shoulderIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_shoulder").Unwrap().transform;

    forearm = childDictionary.modelChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;
    shoulder = childDictionary.modelChildren.GetValue($"b_{hand}_shoulder").Unwrap().transform;
  }

  void FixedUpdate()
  {
    if (forearm.gameObject.HasComponent<ConfigurableJoint>())
    {
      var fc = forearm.GetComponent<ConfigurableJoint>();
      SetTargetRotationInternal(fc, forearmIK.localRotation, forearm.localRotation, Space.Self);

      var hc = humerus.GetComponent<ConfigurableJoint>();
      SetTargetRotationInternal(hc, humerusIK.localRotation, humerus.localRotation, Space.Self);
    }
  }

  void Update()
  {

    if (!forearm.gameObject.HasComponent<ConfigurableJoint>())
    {
      forearm.localRotation = Quaternion.Slerp(forearm.localRotation, forearmIK.localRotation, strength);
      humerus.localRotation = Quaternion.Slerp(humerus.localRotation, humerusIK.localRotation, strength);
      shoulder.localRotation = Quaternion.Slerp(shoulder.localRotation, shoulderIK.localRotation, strength);
    }
  }


  static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
  {
    // Calculate the rotation expressed by the joint's axis and secondary axis
    var right = joint.axis;
    var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
    var up = Vector3.Cross(forward, right).normalized;
    Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

    // Transform into world space
    Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

    // Counter-rotate and apply the new local rotation.
    // Joint space is the inverse of world space, so we need to invert our value
    if (space == Space.World)
    {
      resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
    }
    else
    {
      resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
    }

    // Transform back into joint space
    resultRotation *= worldToJointSpace;

    // Set target rotation to our newly calculated rotation
    joint.targetRotation = resultRotation;
  }
}
