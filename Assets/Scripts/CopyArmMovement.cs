using System;
using System.Linq;
using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1F / 60F;
  public GameObject targetArm;
  private ChildDictionary targetChildDictionary;
  private Tuple<Transform, Transform>[] pairedChildrenToCopyTransforms;
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

    pairedChildrenToCopyTransforms = targetChildDictionary.modelChildren.Values.Where(child =>
    {
      return BoneNameOperations.IsTrackedBone(child.name) && child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
    }).Select((child) => new Tuple<Transform, Transform>(
      child.transform,
      childDictionary.modelChildren.GetValue(child.name).Unwrap().transform)).ToArray();

    var handPrefix = handedness.HandPrefix();

    forearm = childDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

    forearmOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerusOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

  }

  void Update()
  {

    // Copy IK from other arm
    forearm.localRotation = Quaternion.Slerp(forearmOther.localRotation, forearm.localRotation, strength);
    humerus.localRotation = Quaternion.Slerp(humerusOther.localRotation, humerus.localRotation, strength);

    foreach (var sourceAndDestination in pairedChildrenToCopyTransforms)
    {
      var current = sourceAndDestination.Item2;
      var source = sourceAndDestination.Item1;
      current.LerpLocal(source, strength);
    }
  }
}
