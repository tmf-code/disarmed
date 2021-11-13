using System;
using UnityEngine;

public class DataSources : MonoBehaviour
{
  public ArmBoneData ikArmBoneData;
  public HandBoneData trackingHandBoneData;
  public HandRootData trackingHandRootData;
  public GestureData gestureData;
  public Handedness handedness;
  public SharedFrameBuffer sharedFrameBuffer;

  void Awake()
  {
    handedness = gameObject.GetComponentOrThrow<Handedness>();

    var trackingSide = GameObject.Find("Tracking")
          .GetComponentOrThrow<Tracking>()
          .GetSide(handedness);

    sharedFrameBuffer = trackingSide.GetComponentOrThrow<SharedFrameBuffer>();
    ikArmBoneData = trackingSide
      .GetComponentOrThrow<InverseKinematics>()
      .GetArmBoneData();

    var handTracking = trackingSide
      .GetComponentOrThrow<HandTracking>();

    trackingHandBoneData = handTracking
      .GetHandBoneData();

    trackingHandRootData = handTracking
      .GetHandRootData();

    gestureData = trackingSide
      .GetComponent<GestureState>()
      .GetGestureData();

    // JP - Left here in case the model hierarchy changes. Use this then you can copy paste the values
    // From runtime into editor time
    // modelChildren = new StringGameObjectDictionary();
    // vrTrackingDataChildren = new StringGameObjectDictionary();

    // modelChildren.CopyFrom(model.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));

    // vrTrackingDataChildren.CopyFrom(vrTrackingData.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));
  }
}

[Serializable]
public class LocalRotation
{
  [SerializeField]
  private Transform source;
  public Quaternion localRotation { get => source.localRotation; }
  public LocalRotation(Transform source) => this.source = source;
}

[Serializable]
public class HandRootData
{
  public Transform handRoot;
  public HandRootData(Transform handRoot) => this.handRoot = handRoot;
}

[Serializable]
public class ArmBoneData
{
  public LocalRotation forearm;
  public LocalRotation humerus;
  public LocalRotation shoulder;

  public ArmBoneData(Transform forearm, Transform humerus, Transform shoulder)
  {
    this.forearm = new LocalRotation(forearm);
    this.humerus = new LocalRotation(humerus);
    this.shoulder = new LocalRotation(shoulder);
  }
}

[Serializable]
public class StringLocalRotationDictionary : SerializableDictionary<string, LocalRotation> { }

public class HandBoneData
{
  public StringLocalRotationDictionary bones;
  public HandBoneData(StringLocalRotationDictionary bones) => this.bones = bones;
}