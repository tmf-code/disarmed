using System;
using System.Collections.Generic;
using UnityEngine;

public class SavePose : MonoBehaviour
{
  public Transform start;

  public string filename;

  [SerializeField]
  private string json;

  public void Save()
  {
    var transforms = new List<SerializedTransform>();

    void Serialize(Transform transform) => transforms.Add(new SerializedTransform(transform));

    Serialize(start);
    start.TraverseChildren(Serialize);

    var serializedTransforms = new SerializedTransforms(transforms.ToArray());
    json = JsonUtility.ToJson(serializedTransforms);
    var filepath = $"{Application.dataPath}/Resources/Poses/{filename}.json";
    Debug.Log($"Writing to {filepath}");
    System.IO.File.WriteAllText(filepath, json);
  }
}

[Serializable]
public class SerializedTransforms
{
  public SerializedTransform[] transforms;

  public SerializedTransforms(SerializedTransform[] transforms)
  {
    this.transforms = transforms;
  }
}

[Serializable]
public class SerializedTransform
{
  public Vector3 localPosition;
  public Quaternion localRotation;
  public Vector3 localScale;
  public string name;

  public SerializedTransform(Transform transform)
  {
    localPosition = transform.localPosition;
    localRotation = transform.localRotation;
    localScale = transform.localScale;
    name = transform.name;
  }
}