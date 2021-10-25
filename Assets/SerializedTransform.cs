using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedTransform
{
  /// <summary>
  /// Ordered elements, localPosition, localRotation, localScale
  /// To reduce JSON size
  /// </summary>
  public List<double> data;
  /// <summary>
  /// Name
  /// </summary>
  public string n;

  [NonSerialized] public UnSerializedTransform unSerialized;

  public SerializedTransform(Transform transform)
  {
    data = new List<double>(10) {
      transform.localPosition.x,
      transform.localPosition.y,
      transform.localPosition.z,
      transform.localRotation.x,
      transform.localRotation.y,
      transform.localRotation.z,
      transform.localRotation.w,
      transform.localScale.x,
      transform.localScale.y,
      transform.localScale.z,
     };
    data = data.ConvertAll(data => Math.Round(data, 4));
    n = transform.name;
  }
  public void OnAfterDeserialize() => unSerialized = new UnSerializedTransform(this);

  public Vector3 localPosition => new Vector3((float)data[0], (float)data[1], (float)data[2]);
  public Quaternion localRotation => new Quaternion((float)data[3], (float)data[4], (float)data[5], (float)data[6]);
  public Vector3 localScale => new Vector3((float)data[7], (float)data[8], (float)data[9]);
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

public class UnSerializedTransform
{
  public Vector3 localPosition;
  public Quaternion localRotation;
  public Vector3 localScale;
  public string name;

  public UnSerializedTransform(SerializedTransform transform)
  {
    localPosition = transform.localPosition;
    localRotation = transform.localRotation;
    localScale = transform.localScale;
    name = transform.n;
  }
}
