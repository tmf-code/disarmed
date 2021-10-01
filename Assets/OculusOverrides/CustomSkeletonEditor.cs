using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using BoneId = Skeleton.BoneId;

[CustomEditor(typeof(CustomSkeleton))]
public class CustomSkeletonEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawPropertiesExcluding(serializedObject, new string[] { "_customBones" });
    serializedObject.ApplyModifiedProperties();

    CustomSkeleton skeleton = (CustomSkeleton)target;
    Skeleton.SkeletonType skeletonType = skeleton.GetSkeletonType();

    if (skeletonType == Skeleton.SkeletonType.None)
    {
      EditorGUILayout.HelpBox("Please select a SkeletonType.", MessageType.Warning);
    }
    else
    {
      if (GUILayout.Button("Auto Map Bones"))
      {
        skeleton.TryAutoMapBonesByName();
        EditorUtility.SetDirty(skeleton);
        EditorSceneManager.MarkSceneDirty(skeleton.gameObject.scene);
      }

      EditorGUILayout.LabelField("Bones", EditorStyles.boldLabel);
      BoneId start = skeleton.GetCurrentStartBoneId();
      BoneId end = skeleton.GetCurrentEndBoneId();
      if (start != BoneId.Invalid && end != BoneId.Invalid)
      {
        for (int i = (int)start; i < (int)end; ++i)
        {
          string boneName = Skeleton.BoneLabelFromBoneId(skeletonType, (BoneId)i);
          skeleton.CustomBones[i] = (Transform)EditorGUILayout.ObjectField(
              boneName,
              skeleton.CustomBones[i],
              typeof(Transform),
              true
          );
        }
      }
    }
  }
}

