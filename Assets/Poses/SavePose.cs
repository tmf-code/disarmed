using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SavePose : MonoBehaviour
{
  public Transform start;

  public string filename;

  [SerializeField]
  private string json;

  void Start()
  {
    start = transform.FindChildRecursive("VRTrackingData");
  }

  public void Save()
  {
    start = transform.FindChildRecursive("VRTrackingData");
    var transforms = new List<SerializedTransform>();

    void Serialize(Transform transform)
    {
      if (!BoneNameToBoneId.IsTrackedBone(transform.name)) return;
      transforms.Add(new SerializedTransform(transform));
    }

    Serialize(start);

    start.TraverseChildren(Serialize);

    var serializedTransforms = new SerializedTransforms(transforms);
    json = JsonUtility.ToJson(serializedTransforms);
    var filepath = $"{Application.dataPath}/Resources/Poses/{filename}.json";
    Debug.Log($"Writing to {filepath}");
    System.IO.File.WriteAllText(filepath, json);
    AssetDatabase.Refresh();
  }
}
