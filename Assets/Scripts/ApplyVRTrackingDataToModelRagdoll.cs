using UnityEngine;

public class ApplyVRTrackingDataToModelRagdoll : MonoBehaviour
{
  private Transform forearmIK;
  private Transform humerusIK;

  private Transform forearm;
  private Transform humerus;

  private Transform trackingData;
  private Rigidbody model;

  [SerializeField]
  private TransformPair[] handBonePairs;

  [Range(0, 1)]
  public float strength = 10F / 60F;

  private ChildDictionary childDictionary;
  private ConfigurableJoint forearmJoint;
  private ConfigurableJoint humerusJoint;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var hand = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    forearmIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerusIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;

    forearm = childDictionary.modelChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;

    forearmJoint = forearm.GetComponent<ConfigurableJoint>();
    humerusJoint = humerus.GetComponent<ConfigurableJoint>();

    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model").gameObject.GetComponentOrThrow<Rigidbody>();

    handBonePairs = childDictionary.handBonePairs;
  }

  void FixedUpdate()
  {
    // Make the IK joints move
    SetTargetRotationInternal(forearmJoint, forearmIK.localRotation, forearm.localRotation, Space.Self);
    SetTargetRotationInternal(humerusJoint, humerusIK.localRotation, humerus.localRotation, Space.Self);

    // Make the base move
    var modelForceDirection = Quaternion.Inverse(model.transform.localRotation) * trackingData.localRotation;
    var modelRigidBody = model.GetComponent<Rigidbody>();
    modelRigidBody.AddRelativeTorque(new Vector3(modelForceDirection[0], modelForceDirection[1], modelForceDirection[2]) * 500F);

    // Make the fingers move
    foreach (var pair in handBonePairs)
    {
      var current = pair.Item2;
      current.LerpLocal(pair.Item1, strength);
    }
  }

  private static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
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
