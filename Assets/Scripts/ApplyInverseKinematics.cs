using UnityEngine;
/// <summary>
/// Applies Tracking IK Components to model
/// </summary>
public class ApplyInverseKinematics : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private LocalRotation forearmIK;
  private LocalRotation humerusIK;
  private LocalRotation shoulderIK;
  private Transform forearm;
  private Transform humerus;
  private Transform shoulder;

  void Start()
  {
    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var data = gameObject.GetComponentOrThrow<DataSources>().ikArmBoneData;

    forearmIK = data.forearm;
    humerusIK = data.humerus;
    shoulderIK = data.shoulder;

    forearm = childDictionary.modelForearm;
    humerus = childDictionary.modelHumerus;
    shoulder = childDictionary.modelShoulder;
  }

  void Update()
  {
    forearm.localRotation = forearmIK.localRotation;
    humerus.localRotation = humerusIK.localRotation;
    // shoulder.localRotation = shoulderIK.localRotation;

    // forearm.localRotation = Quaternion.SlerpUnclamped(forearm.localRotation, forearmIK.localRotation, strength);
    // humerus.localRotation = Quaternion.SlerpUnclamped(humerus.localRotation, humerusIK.localRotation, strength);
    // shoulder.localRotation = Quaternion.SlerpUnclamped(shoulder.localRotation, shoulderIK.localRotation, strength);
  }
}
