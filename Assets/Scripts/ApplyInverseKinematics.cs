using UnityEngine;
/// <summary>
/// Applies VRTrackingData IK Components to model
/// </summary>
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
  void Update()
  {
    forearm.localRotation = Quaternion.SlerpUnclamped(forearm.localRotation, forearmIK.localRotation, strength);
    humerus.localRotation = Quaternion.SlerpUnclamped(humerus.localRotation, humerusIK.localRotation, strength);
    shoulder.localRotation = Quaternion.SlerpUnclamped(shoulder.localRotation, shoulderIK.localRotation, strength);
  }
}
