using System.Linq;
using UnityEngine;

public class ApplyVRTrackingDataToModelRagdoll : MonoBehaviour
{
  private LocalRotation forearmIK;
  private LocalRotation humerusIK;

  private Transform forearm;
  private Transform humerus;

  private Transform trackingData;
  private Rigidbody model;

  [SerializeField]
  private LocalRotationTransformPair[] handBonePairs;

  [Range(0, 1)]
  public float strength = 10F / 60F;

  private ChildDictionary childDictionary;
  private ConfigurableJoint forearmJoint;
  private ConfigurableJoint humerusJoint;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var hand = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    forearm = childDictionary.modelForearm;
    humerus = childDictionary.modelHumerus;

    forearmJoint = forearm.gameObject.GetComponentOrThrow<ConfigurableJoint>();
    humerusJoint = humerus.gameObject.GetComponentOrThrow<ConfigurableJoint>();

    var dataSources = gameObject.GetComponentOrThrow<DataSources>();

    trackingData = dataSources.trackingHandRootData.handRoot;
    model = childDictionary.model.gameObject.GetComponentOrThrow<Rigidbody>();

    var ikArmBoneData = dataSources.ikArmBoneData;

    forearmIK = ikArmBoneData.forearm;
    humerusIK = ikArmBoneData.humerus;


    var handBoneData = dataSources.trackingHandBoneData.bones;
    handBonePairs = handBoneData.Select(nameGameObjectKV =>
    {
      var source = nameGameObjectKV.Value;
      var name = nameGameObjectKV.Key;
      var destination = childDictionary.modelChildren.GetValue(name).Unwrap().transform;
      return new LocalRotationTransformPair(source, destination);
    }).ToArray();
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
      var current = pair.transform;
      current.localRotation = Quaternion.SlerpUnclamped(current.localRotation, pair.rotation.localRotation, strength);
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
