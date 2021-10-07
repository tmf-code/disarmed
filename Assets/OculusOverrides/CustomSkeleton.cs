using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class CustomSkeleton : Skeleton
{
  private static BoneName FbxBoneNameFromBoneId(HandTypes handType, TrackedBones boneId)
  {
    {
      var isFingerTipMarker = boneId >= TrackedBones.Hand_ThumbTip && boneId <= TrackedBones.Hand_PinkyTip;
      var handSideprefix = handType == HandTypes.HandLeft ? "l_" : "r_";
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
  }

  public Bone GetBoneFromBoneName(BoneName name)
  {
    var boneId = BoneNameToBoneId.GetTrackedBone(name);
    var maybeBone = bones.Find(bone => bone.id == boneId);

    return maybeBone;
  }

  protected override void InitializeBones()
  {
    if (bones == null || bones.Count != skeleton.NumBones)
    {
      bones = new List<Bone>(new Bone[skeleton.NumBones]);
      Bones = bones.AsReadOnly();
    }

    var armature = transform.FindRecursiveOrThrow("Armature");
    var vrTrackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    var maybeCopyArmature = vrTrackingData.Find("Armature");
    var copyArmature = maybeCopyArmature != null ? maybeCopyArmature : Instantiate(armature, vrTrackingData);
    copyArmature.name = "Armature";

    for (int i = 0; i < bones.Count; ++i)
    {
      Bone bone = bones[i] ?? (bones[i] = new Bone());
      bone.id = (TrackedBones)i;

      BoneName fbxBoneName = FbxBoneNameFromBoneId(HandType, bone.id);
      Transform boneTransform = copyArmature.FindRecursiveOrThrow(fbxBoneName.ToString());

      bone.transform = boneTransform;
      bone.transform.localRotation = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();
    }
  }

  private static readonly string[] HandBoneNames =
  {
        "wrist",
        "forearm_stub",
        "thumb0",
        "thumb1",
        "thumb2",
        "thumb3",
        "index1",
        "index2",
        "index3",
        "middle1",
        "middle2",
        "middle3",
        "ring1",
        "ring2",
        "ring3",
        "pinky0",
        "pinky1",
        "pinky2",
        "pinky3"
    };

  private static readonly string[] HandFingerNames =
  {
        "thumb",
        "index",
        "middle",
        "ring",
        "pinky"
    };
}
