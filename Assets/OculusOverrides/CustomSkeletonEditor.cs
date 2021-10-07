using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using BoneId = Skeleton.BoneId;

[CustomEditor(typeof(CustomSkeleton))]
public class CustomSkeletonEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawPropertiesExcluding(serializedObject, new string[] { "boneTransforms" });
    serializedObject.ApplyModifiedProperties();

    CustomSkeleton skeleton = (CustomSkeleton)target;

    if (GUILayout.Button("Auto Map Bones"))
    {
      skeleton.TryAutoMapBonesByName();
      EditorUtility.SetDirty(skeleton);
      EditorSceneManager.MarkSceneDirty(skeleton.gameObject.scene);
    }

    EditorGUILayout.LabelField("Bones", EditorStyles.boldLabel);
    BoneId start = BoneId.Hand_Start;
    BoneId end = BoneId.Hand_End;

    for (int i = (int)start; i < (int)end; ++i)
    {
      string boneName = BoneLabelFromBoneId((BoneId)i);
      skeleton.BoneTransforms[i] = (Transform)EditorGUILayout.ObjectField(
          boneName,
          skeleton.BoneTransforms[i],
          typeof(Transform),
          true
      );
    }

  }

  public static string BoneLabelFromBoneId(BoneId boneId)
  {
    return boneId switch
    {
      BoneId.Hand_WristRoot => "Hand_WristRoot",
      BoneId.Hand_ForearmStub => "Hand_ForearmStub",
      BoneId.Hand_Thumb0 => "Hand_Thumb0",
      BoneId.Hand_Thumb1 => "Hand_Thumb1",
      BoneId.Hand_Thumb2 => "Hand_Thumb2",
      BoneId.Hand_Thumb3 => "Hand_Thumb3",
      BoneId.Hand_Index1 => "Hand_Index1",
      BoneId.Hand_Index2 => "Hand_Index2",
      BoneId.Hand_Index3 => "Hand_Index3",
      BoneId.Hand_Middle1 => "Hand_Middle1",
      BoneId.Hand_Middle2 => "Hand_Middle2",
      BoneId.Hand_Middle3 => "Hand_Middle3",
      BoneId.Hand_Ring1 => "Hand_Ring1",
      BoneId.Hand_Ring2 => "Hand_Ring2",
      BoneId.Hand_Ring3 => "Hand_Ring3",
      BoneId.Hand_Pinky0 => "Hand_Pinky0",
      BoneId.Hand_Pinky1 => "Hand_Pinky1",
      BoneId.Hand_Pinky2 => "Hand_Pinky2",
      BoneId.Hand_Pinky3 => "Hand_Pinky3",
      BoneId.Hand_ThumbTip => "Hand_ThumbTip",
      BoneId.Hand_IndexTip => "Hand_IndexTip",
      BoneId.Hand_MiddleTip => "Hand_MiddleTip",
      BoneId.Hand_RingTip => "Hand_RingTip",
      BoneId.Hand_PinkyTip => "Hand_PinkyTip",
      _ => throw new System.Exception("BoneID {boneId} does not resolve to a BoneLabel. Would previously return 'Hand_Unknown'"),
    };
  }
}

