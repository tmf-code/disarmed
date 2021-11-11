using System.Linq;
using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 30F / 60F;
  public GameObject targetArm;
  private ChildDictionary targetChildDictionary;
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;
  private ChildDictionary childDictionary;
  private Handedness handedness;
  private Transform forearm;
  private Transform humerus;
  private Transform forearmOther;
  private Transform humerusOther;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    handedness = gameObject.GetComponentOrThrow<Handedness>();

    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    targetArm = handedness.handType switch
    {
      Handedness.HandTypes.HandLeft => playerArms.left,
      _ => playerArms.right,
    };

    targetChildDictionary = targetArm.GetComponentOrThrow<ChildDictionary>();

    handBonePairs = targetChildDictionary.modelChildren.Values.Where(child =>
   {
     var isTrackedBone = BoneNameOperations.IsTrackedBone(child.name);
     var isNotIKBone = child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
     var isHandBone = isTrackedBone && isNotIKBone;
     return isHandBone;
   }).Select(bone =>
   {
     var modelBoneTransform = childDictionary.modelChildren.GetValue(bone.name).Unwrap().transform;
     return new TransformPair(bone.transform, modelBoneTransform);
   }).ToArray();

    var handPrefix = handedness.HandPrefix();

    forearm = childDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

    forearmOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerusOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

  }

  void Update()
  {
    // Copy IK from other arm
    forearm.localRotation = Quaternion.SlerpUnclamped(forearmOther.localRotation, forearm.localRotation, strength);
    humerus.localRotation = Quaternion.SlerpUnclamped(humerusOther.localRotation, humerus.localRotation, strength);

    foreach (var sourceAndDestination in handBonePairs)
    {
      var source = sourceAndDestination.Item1;
      var current = sourceAndDestination.Item2;
      current.LerpLocal(source, strength);
    }
  }
}
