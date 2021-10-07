using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class CustomSkeleton : Skeleton
{

  [HideInInspector]
  [SerializeField]
  private List<Transform> _customBones_V2 = new List<Transform>(new Transform[(int)BoneId.Max]);

#if UNITY_EDITOR

  private static readonly string[] _fbxHandSidePrefix = { "l_", "r_" };
  private static readonly string _fbxHandBonePrefix = "b_";

  private static readonly string[] _fbxHandBoneNames =
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

  private static readonly string[] _fbxHandFingerNames =
  {
        "thumb",
        "index",
        "middle",
        "ring",
        "pinky"
    };
#endif

  public List<Transform> CustomBones
  {
    get { return _customBones_V2; }
  }

#if UNITY_EDITOR
  public void TryAutoMapBonesByName()
  {
    BoneId start = GetCurrentStartBoneId();
    BoneId end = GetCurrentEndBoneId();
    SkeletonType skeletonType = GetSkeletonType();

    var boneIdsAreInvalid = start == BoneId.Invalid || end == BoneId.Invalid;
    if (boneIdsAreInvalid) return;

    for (int boneId = (int)start; boneId < (int)end; ++boneId)
    {
      string fbxBoneName = FbxBoneNameFromBoneId(skeletonType, (BoneId)boneId);
      Transform boneTransform = transform.FindRecursiveOrThrow(fbxBoneName);

      _customBones_V2[boneId] = boneTransform;
    }
  }

  private static string FbxBoneNameFromBoneId(SkeletonType skeletonType, BoneId boneId)
  {
    {
      var isFingerTipMarker = boneId >= BoneId.Hand_ThumbTip && boneId <= BoneId.Hand_PinkyTip;
      if (isFingerTipMarker)
      {
        var fingerNameIndex = (int)boneId - (int)BoneId.Hand_ThumbTip;
        return _fbxHandSidePrefix[(int)skeletonType]
            + _fbxHandFingerNames[fingerNameIndex]
            + "_finger_tip_marker";
      }
      else
      {
        return _fbxHandBonePrefix
            + _fbxHandSidePrefix[(int)skeletonType]
            + _fbxHandBoneNames[(int)boneId];
      }
    }
  }
#endif

  public Bone GetBoneFromBoneName(string name)
  {
    var boneId = BoneNameToBoneId.getBoneId(name);
    var maybeBone = _bones.Find(bone => bone.Id == boneId);

    return maybeBone;
  }

  protected override void InitializeBones()
  {
    bool flipX = (
        _skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight
    );

    if (_bones == null || _bones.Count != _skeleton.NumBones)
    {
      _bones = new List<Bone>(new Bone[_skeleton.NumBones]);
      Bones = _bones.AsReadOnly();
    }

    for (int i = 0; i < _bones.Count; ++i)
    {
      Bone bone = _bones[i] ?? (_bones[i] = new Bone());
      bone.Id = (BoneId)_skeleton.Bones[i].Id;
      bone.ParentBoneIndex = _skeleton.Bones[i].ParentBoneIndex;
      bone.transform = _customBones_V2[(int)bone.Id];

      bone.transform.localRotation = flipX
          ? _skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf()
          : _skeleton.Bones[i].Pose.Orientation.FromFlippedZQuatf();
    }

    var armature = transform.FindRecursiveOrThrow("Armature");

    var maybeCopyArmature = transform.FindChildRecursive("copy_Armature");
    var copyArmature = maybeCopyArmature != null ? maybeCopyArmature : Instantiate(armature, armature.parent.transform);
    copyArmature.name = "copy_Armature";

    for (int i = 0; i < _bones.Count; ++i)
    {
      Bone bone = _bones[i];
      var copy = copyArmature.FindRecursiveOrThrow(bone.transform.name);
      bone.alwaysUpdatesTransform = copy;
    }
  }
}
