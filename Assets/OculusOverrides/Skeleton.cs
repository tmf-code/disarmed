using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class Skeleton : MonoBehaviour
{
  private Handedness handedness;
  private CustomHand handDataProvider;

  public bool updateRootScale = false;
  public bool updateRootPose = false;
  public bool updateBones = false;

  protected List<Bone> bones;

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
      transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
      transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
    }

    if (updateRootScale)
    {
      transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
    }

    for (var i = 0; i < bones.Count; ++i)
    {
      var bone = bones[i];
      if (bone.transform == null) continue;

      var quaternion = data.BoneRotations[i].FromFlippedXQuatf();
      var isBoneWristRoot = bone.Id == BoneId.Hand_WristRoot;
      if (isBoneWristRoot)
      {
        quaternion *= wristFixupRotation;
      }
      bone.localRotation = quaternion;
    }

    for (var i = 0; i < bones.Count; ++i)
    {
      var bone = bones[i];
      bone.Update(updateBones);
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

  public enum BoneId
  {
    // hand bones
    Hand_Start = OVRPlugin.BoneId.Hand_Start,
    Hand_WristRoot = OVRPlugin.BoneId.Hand_WristRoot, // root frame of the hand, where the wrist is located
    Hand_ForearmStub = OVRPlugin.BoneId.Hand_ForearmStub, // frame for user's forearm
    Hand_Thumb0 = OVRPlugin.BoneId.Hand_Thumb0, // thumb trapezium bone
    Hand_Thumb1 = OVRPlugin.BoneId.Hand_Thumb1, // thumb metacarpal bone
    Hand_Thumb2 = OVRPlugin.BoneId.Hand_Thumb2, // thumb proximal phalange bone
    Hand_Thumb3 = OVRPlugin.BoneId.Hand_Thumb3, // thumb distal phalange bone
    Hand_Index1 = OVRPlugin.BoneId.Hand_Index1, // index proximal phalange bone
    Hand_Index2 = OVRPlugin.BoneId.Hand_Index2, // index intermediate phalange bone
    Hand_Index3 = OVRPlugin.BoneId.Hand_Index3, // index distal phalange bone
    Hand_Middle1 = OVRPlugin.BoneId.Hand_Middle1, // middle proximal phalange bone
    Hand_Middle2 = OVRPlugin.BoneId.Hand_Middle2, // middle intermediate phalange bone
    Hand_Middle3 = OVRPlugin.BoneId.Hand_Middle3, // middle distal phalange bone
    Hand_Ring1 = OVRPlugin.BoneId.Hand_Ring1, // ring proximal phalange bone
    Hand_Ring2 = OVRPlugin.BoneId.Hand_Ring2, // ring intermediate phalange bone
    Hand_Ring3 = OVRPlugin.BoneId.Hand_Ring3, // ring distal phalange bone
    Hand_Pinky0 = OVRPlugin.BoneId.Hand_Pinky0, // pinky metacarpal bone
    Hand_Pinky1 = OVRPlugin.BoneId.Hand_Pinky1, // pinky proximal phalange bone
    Hand_Pinky2 = OVRPlugin.BoneId.Hand_Pinky2, // pinky intermediate phalange bone
    Hand_Pinky3 = OVRPlugin.BoneId.Hand_Pinky3, // pinky distal phalange bone
    Hand_MaxSkinnable = OVRPlugin.BoneId.Hand_MaxSkinnable,
    // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
    // NOTE: Hand_ThumbTip == Hand_MaxSkinnable since the extended tips need to be contiguous
    Hand_ThumbTip = OVRPlugin.BoneId.Hand_ThumbTip, // tip of the thumb
    Hand_IndexTip = OVRPlugin.BoneId.Hand_IndexTip, // tip of the index finger
    Hand_MiddleTip = OVRPlugin.BoneId.Hand_MiddleTip, // tip of the middle finger
    Hand_RingTip = OVRPlugin.BoneId.Hand_RingTip, // tip of the ring finger
    Hand_PinkyTip = OVRPlugin.BoneId.Hand_PinkyTip, // tip of the pinky
    Hand_End = OVRPlugin.BoneId.Hand_End,
    // add new bones here

    Max = OVRPlugin.BoneId.Max
  }
}

public class Bone
{
  public Skeleton.BoneId Id { get; set; }
  public short ParentBoneIndex { get; set; }

#nullable enable
  public Transform? transform;
  public Transform? alwaysUpdatesTransform;
#nullable disable

  public Quaternion localRotation = Quaternion.identity;

  public void Update(bool updateMaster)
  {
    if (updateMaster)
    {
      transform.localRotation = localRotation;
    }
    alwaysUpdatesTransform.localPosition = transform.localPosition;
    alwaysUpdatesTransform.localRotation = localRotation;
  }
}