using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PoseSelector : MonoBehaviour
{
  public string posePath = "Poses";
  public Pose[] poses = new Pose[] { };

  void Start()
  {
    Load();
  }

  public void Load()
  {
    AssetDatabase.Refresh();

    poses = new Pose[] { };
    var textAssets = Resources.LoadAll(posePath);
    foreach (var item in textAssets)
    {
      if (item.GetType() == typeof(TextAsset))
      {
        TextAsset textAsset = item as TextAsset;
        var serializedTransforms = JsonUtility.FromJson<SerializedTransforms>(textAsset.text);
        var pose = new Pose(item.name, serializedTransforms);
        poses = poses.Append(pose).ToArray();
      }
    }
  }
}
[Serializable]
public class Pose
{
  public string name;
  public Dictionary<string, SerializedTransform> transforms;

  private static Dictionary<string, SerializedTransform> FromSerializedTransforms(SerializedTransforms transforms)
  {
    Dictionary<string, SerializedTransform> dictionary = new Dictionary<string, SerializedTransform>();
    foreach (SerializedTransform transform in transforms.transforms)
    {
      dictionary.Add(transform.name, transform);
    }

    return dictionary;
  }

  public Pose(string name, SerializedTransforms transforms)
  {
    this.name = name;
    this.transforms = FromSerializedTransforms(transforms);
  }
}
