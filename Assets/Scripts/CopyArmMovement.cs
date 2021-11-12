using System;
using System.Collections.Generic;
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

  public int frameDelay = 0;

  readonly Queue<FrameBuffer> frameQueue = new Queue<FrameBuffer>();

  void FixedUpdate()
  {
    frameQueue.Enqueue(new FrameBuffer(forearmOther.localRotation, humerusOther.localRotation, handBonePairs));
    if (frameQueue.Count > frameDelay)
    {
      var frameToApply = frameQueue.Dequeue();
      // Copy IK from other arm
      forearm.localRotation = Quaternion.SlerpUnclamped(forearm.localRotation, frameToApply.forearmOther, strength);
      humerus.localRotation = Quaternion.SlerpUnclamped(humerus.localRotation, frameToApply.humerusOther, strength);

      foreach (var sourceAndDestination in frameToApply.handBonePairs)
      {
        var source = sourceAndDestination.source;
        var destination = sourceAndDestination.destination;
        destination.LerpLocal(source, strength);
      }
    }
  }
}

public class FrameBuffer
{
  public Quaternion forearmOther;
  public Quaternion humerusOther;
  public ITransformPair[] handBonePairs;

  public FrameBuffer(Quaternion forearmOther, Quaternion Other, TransformPair[] handBonePairs)
  {
    this.forearmOther = forearmOther;
    this.humerusOther = Other;

    this.handBonePairs = new ITransformPair[handBonePairs.Length];
    for (var pairIndex = 0; pairIndex < handBonePairs.Length; pairIndex++)
    {
      var sourceAndDestination = handBonePairs[pairIndex];
      var source = sourceAndDestination.Item1;
      var current = sourceAndDestination.Item2;

      UnSerializedTransform transform = new UnSerializedTransform(source.localPosition, source.localRotation, source.localScale);
      var pair = new ITransformPair(transform, current);
      this.handBonePairs[pairIndex] = pair;
    }
  }
}

[Serializable]
public struct ITransformPair
{
  public ITransform source;
  public Transform destination;

  public ITransformPair(ITransform source, Transform destination)
  {
    this.source = source;
    this.destination = destination;
  }
}
