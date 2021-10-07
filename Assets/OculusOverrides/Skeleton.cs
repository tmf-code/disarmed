using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class Skeleton : MonoBehaviour
{
  private Handedness handedness;
  private CustomHand handDataProvider;
  private Transform vrTrackingDataTransform;

  public bool updateRootScale = false;
  public bool updateRootPose = false;

  [HideInInspector]
  [SerializeField]
  protected List<Bone> bones = new List<Bone>();

  protected OVRPlugin.Skeleton2 skeleton = new OVRPlugin.Skeleton2();
  private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

  public bool IsInitialized { get; private set; }
  public bool IsDataValid { get; private set; }
  public bool IsDataHighConfidence { get; private set; }
  public IList<Bone> Bones { get; protected set; }
  public IList<Bone> BindPoses { get; private set; }
  public int SkeletonChangedCount { get; private set; }
  public HandTypes HandType
  {
    get
    {
      handedness = gameObject.GetComponentIfNull(handedness);
      return handedness.handType;
    }
  }

  private void Awake()
  {
    handDataProvider = gameObject.GetComponentIfNull(handDataProvider);
    handedness = gameObject.GetComponentIfNull(handedness);
    vrTrackingDataTransform = transform.FindOrThrow("VRTrackingData");
    bones = new List<Bone>();
    Bones = bones.AsReadOnly();
  }

  private void Start()
  {
    if (ShouldInitialize()) Initialize();
  }

  private bool ShouldInitialize()
  {
    if (IsInitialized) return false;

#if UNITY_EDITOR
    return OVRInput.IsControllerConnected(OVRInput.Controller.Hands);
#else
    return true;
#endif
  }


  private void Initialize()
  {
    if (!OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)HandType, ref skeleton)) return;

    InitializeBones();
    IsInitialized = true;
  }

  protected virtual void InitializeBones()
  {
    throw new NotImplementedException("Should not create base class Skeleton. If you need this look under the oculus package");
  }

  private void Update()
  {
#if UNITY_EDITOR
    if (ShouldInitialize()) Initialize();
#endif

    if (!IsInitialized || handDataProvider == null)
    {
      IsDataValid = false;
      IsDataHighConfidence = false;
      return;
    }

    var data = handDataProvider.GetSkeletonPoseData();

    IsDataValid = data.IsDataValid;

    if (!data.IsDataValid) return;

    if (SkeletonChangedCount != data.SkeletonChangedCount)
    {
      SkeletonChangedCount = data.SkeletonChangedCount;
      IsInitialized = false;
      Initialize();
    }

    IsDataHighConfidence = data.IsDataHighConfidence;

    if (updateRootPose)
    {
      vrTrackingDataTransform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
      vrTrackingDataTransform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
    }

    if (updateRootScale) vrTrackingDataTransform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);

    for (var i = 0; i < bones.Count; ++i)
    {
      var bone = bones[i];
      if (bone.transform == null) continue;

      var quaternion = data.BoneRotations[i].FromFlippedXQuatf();
      var isBoneWristRoot = bone.id == TrackedBones.Hand_WristRoot;
      if (isBoneWristRoot)
      {
        quaternion *= wristFixupRotation;
      }
      bone.localRotation = quaternion;
    }

    for (var i = 0; i < bones.Count; ++i)
    {
      var bone = bones[i];
      bone.Update();
    }
  }

  private void FixedUpdate()
  {
    if (!IsInitialized || handDataProvider == null)
    {
      IsDataValid = false;
      IsDataHighConfidence = false;
      return;
    }

    Update();
  }
}

public class Bone
{
  public TrackedBones id;
  public Transform transform;
  public Quaternion localRotation = Quaternion.identity;

  public void Update()
  {
    transform.localRotation = localRotation;
  }
}