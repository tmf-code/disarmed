using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharedFrameBuffer : MonoBehaviour
{
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;

  public HandTracking handTracking;
  public InverseKinematics inverseKinematics;

  public int maxFrames = 200;
  public readonly Queue<FrameBuffer> frameQueue = new Queue<FrameBuffer>();

  void FixedUpdate()
  {

    if (frameQueue.Count > maxFrames)
    {
      frameQueue.Dequeue();
    }
    else
    {
      var ikData = inverseKinematics.GetArmBoneData();
      var forearm = ikData.forearm.localRotation;
      var humerus = ikData.humerus.localRotation;
      var model = handTracking.GetHandRootData().handRoot.transform.localRotation;

      var handData = handTracking.GetHandBoneData();

      var hands = new StringQuaternionDictionary();

      hands.CopyFrom(handData.bones.Select(kv => new { key = kv.Key, value = kv.Value.localRotation }).ToDictionary(kv => kv.key, kv => kv.value));
      frameQueue.Enqueue(new FrameBuffer(forearm, humerus, model, hands));
    };
  }
}

[Serializable]
public class StringQuaternionDictionary : SerializableDictionary<string, Quaternion> { }


[Serializable]
public class FrameBuffer
{
  public Quaternion forearm;
  public Quaternion humerus;
  public Quaternion model;
  public StringQuaternionDictionary trackingHandBoneData;

  public FrameBuffer(Quaternion forearm, Quaternion humerus, Quaternion model, StringQuaternionDictionary trackingHandBoneData)
  {
    this.forearm = forearm;
    this.humerus = humerus;
    this.model = model;
    this.trackingHandBoneData = trackingHandBoneData;
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
