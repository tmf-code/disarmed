using System;
using System.Collections.Generic;

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
      dictionary.Add(transform.n, transform);
    }

    return dictionary;
  }

  public Pose(string name, SerializedTransforms transforms)
  {
    this.name = name;
    this.transforms = FromSerializedTransforms(transforms);
  }
}
