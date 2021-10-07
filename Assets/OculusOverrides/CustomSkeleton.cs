using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class CustomSkeleton : Skeleton
{

  [HideInInspector]
  [SerializeField]
  private List<Transform> boneTransforms = new List<Transform>(new Transform[(int)BoneId.Max]);
  public List<Transform> BoneTransforms
  {
    get { return boneTransforms; }
  }

#if UNITY_EDITOR
  public void TryAutoMapBonesByName()
  {
    BoneId start = BoneId.Hand_Start;
    BoneId end = BoneId.Hand_End;

    for (int boneId = (int)start; boneId < (int)end; ++boneId)
    {
      string fbxBoneName = FbxBoneNameFromBoneId(HandType, (BoneId)boneId);
      Transform boneTransform = transform.FindRecursiveOrThrow(fbxBoneName);
      boneTransforms[boneId] = boneTransform;
    }
  }

  private static string FbxBoneNameFromBoneId(HandTypes handType, BoneId boneId)
  {
    {
      var isFingerTipMarker = boneId >= BoneId.Hand_ThumbTip && boneId <= BoneId.Hand_PinkyTip;
      var handSideprefix = handType == HandTypes.HandLeft ? "l_" : "r_";
      if (isFingerTipMarker)
      {
        var fingerNameIndex = (int)boneId - (int)BoneId.Hand_ThumbTip;
        var fingerName = HandFingerNames[fingerNameIndex];
        return $"{handSideprefix}{fingerName}_finger_tip_marker";
      }
      else
      {
        var boneName = HandBoneNames[(int)boneId];
        return $"b_{handSideprefix}{boneName}";
      }
    }
  }
#endif

  public Bone GetBoneFromBoneName(string name)
  {
    var boneId = BoneNameToBoneId.getBoneId(name);
    var maybeBone = bones.Find(bone => bone.Id == boneId);

    return maybeBone;
  }

  protected override void InitializeBones()
  {
    if (bones == null || bones.Count != skeleton.NumBones)
    {
      bones = new List<Bone>(new Bone[skeleton.NumBones]);
      Bones = bones.AsReadOnly();
    }

    for (int i = 0; i < bones.Count; ++i)
    {
      Bone bone = bones[i] ?? (bones[i] = new Bone());
      bone.Id = (BoneId)skeleton.Bones[i].Id;
      bone.ParentBoneIndex = skeleton.Bones[i].ParentBoneIndex;
      bone.transform = boneTransforms[(int)bone.Id];
      bone.transform.localRotation = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();
    }

    var armature = transform.FindRecursiveOrThrow("Armature");

    var maybeCopyArmature = transform.FindChildRecursive("copy_Armature");
    var copyArmature = maybeCopyArmature != null ? maybeCopyArmature : Instantiate(armature, armature.parent.transform);
    copyArmature.name = "copy_Armature";

    for (int i = 0; i < bones.Count; ++i)
    {
      Bone bone = bones[i];
      bone.alwaysUpdatesTransform = copyArmature.FindRecursiveOrThrow(bone.transform.name);
    }
  }

#if UNITY_EDITOR
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
#endif
}
