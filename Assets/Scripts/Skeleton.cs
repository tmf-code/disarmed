using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class Skeleton : MonoBehaviour
{
  private Handedness handedness;
  private CustomHand handDataProvider;
  private CustomHand otherHandDataProvider;
  private Transform vrTrackingDataTransform;

  public bool updateRootScale = true;
  public bool updateRootPose = true;


  [HideInInspector]
  private List<Bone> bones = new List<Bone>();
  private OVRPlugin.Skeleton2 skeleton = new OVRPlugin.Skeleton2();
  private int skeletonChangedCount;
  private bool isInitialized = false;
  private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

  public bool isSwapped = false;

  private CustomHand GetOtherHandDataProvider()
  {
    var arms = transform.parent.gameObject.GetComponentOrThrow<PlayerArms>();
    if (GetHandedness().handType == Handedness.HandTypes.HandLeft)
    {
      return arms.right.GetComponentOrThrow<CustomHand>();
    }
    else
    {
      return arms.left.GetComponentOrThrow<CustomHand>();
    }


  }

  private void Start()
  {
    handDataProvider = gameObject.GetComponentIfNull(handDataProvider);
    otherHandDataProvider = GetOtherHandDataProvider();
    handedness = gameObject.GetComponentIfNull(handedness);
    vrTrackingDataTransform = transform.FindRecursiveOrThrow("VRTrackingData");
    bones = new List<Bone>();

    if (!ShouldInitialize()) Initialize();
  }

  private bool ShouldInitialize()
  {
    if (isInitialized) return false;

#if UNITY_EDITOR
    return OVRInput.IsControllerConnected(OVRInput.Controller.Hands);
#else
    return true;
#endif
  }

  private void Initialize()
  {
    if (!OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)GetHandedness().handType, ref skeleton)) return;

    InitializeBones();
    isInitialized = true;
  }

  private Handedness GetHandedness() => gameObject.GetComponentIfNull(handedness);

  private void InitializeBones()
  {
    if (bones == null || bones.Count != skeleton.NumBones)
    {
      bones = new List<Bone>(new Bone[skeleton.NumBones]);
    }

    var vrTrackingData = transform.FindRecursiveOrThrow("VRTrackingData");

    for (int i = 0; i < bones.Count; ++i)
    {
      Bone bone = bones[i] ?? (bones[i] = new Bone());
      bone.id = (TrackedBones)i;

      BoneName fbxBoneName = FbxBoneNameFromBoneId(GetHandedness(), bone.id);
      Transform boneTransform = vrTrackingData.FindRecursiveOrThrow(fbxBoneName.ToString());

      bone.transform = boneTransform;
      bone.transform.localRotation = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();
    }
  }

  private void Update()
  {
    if (ShouldInitialize()) Initialize();

    if (!isInitialized || handDataProvider == null)
    {
      return;
    }

    var data = !isSwapped ? handDataProvider.GetSkeletonPoseData() : otherHandDataProvider.GetSkeletonPoseData();

    if (!data.IsDataHighConfidence) return;

    if (skeletonChangedCount != data.SkeletonChangedCount)
    {
      skeletonChangedCount = data.SkeletonChangedCount;
      isInitialized = false;
      Initialize();
    }

    if (updateRootPose)
    {
      vrTrackingDataTransform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
      vrTrackingDataTransform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
      if (isSwapped)
      {
        vrTrackingDataTransform.localPosition = new Vector3(
          -vrTrackingDataTransform.localPosition.x,
          vrTrackingDataTransform.localPosition.y,
          vrTrackingDataTransform.localPosition.z);

        var euler = vrTrackingDataTransform.localRotation.eulerAngles;
        vrTrackingDataTransform.localRotation = Quaternion.Euler(-euler.x, -euler.y + 180F, euler.z + 180F);
      }

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
      if (bone.id == TrackedBones.Hand_ForearmStub) continue;
      bone.Update();
    }
  }

  public Bone GetBoneFromBoneName(BoneName name)
  {
    var boneId = BoneNameToBoneId.GetTrackedBone(name).Value;
    var maybeBone = bones.Find(bone => bone.id == boneId);

    return maybeBone;
  }

  private void FixedUpdate()
  {
    if (!isInitialized || handDataProvider == null)
    {
      return;
    }

    Update();
  }

  private static readonly string[] HandBoneNames =
  {
    "wrist", "forearm_stub",
    "thumb0", "thumb1", "thumb2", "thumb3",
    "index1", "index2", "index3",
    "middle1", "middle2", "middle3",
    "ring1", "ring2", "ring3",
    "pinky0", "pinky1", "pinky2", "pinky3"
  };

  private static readonly string[] HandFingerNames =
  {
    "thumb", "index", "middle", "ring", "pinky"
  };

  private static BoneName FbxBoneNameFromBoneId(Handedness handedness, TrackedBones boneId)
  {
    var handSideprefix = handedness.HandPrefix() + "_";
    var isFingerTipMarker = boneId >= TrackedBones.Hand_ThumbTip && boneId <= TrackedBones.Hand_PinkyTip;
    if (isFingerTipMarker)
    {
      var fingerNameIndex = (int)boneId - (int)TrackedBones.Hand_ThumbTip;
      var fingerName = HandFingerNames[fingerNameIndex];
      return (BoneName)Enum.Parse(
        typeof(BoneName),
        $"{handSideprefix}{fingerName}_finger_tip_marker");
    }
    else
    {
      var boneName = HandBoneNames[(int)boneId];
      return (BoneName)Enum.Parse(
        typeof(BoneName),
        $"b_{handSideprefix}{boneName}");
    }
  }

  [Serializable]
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
}

