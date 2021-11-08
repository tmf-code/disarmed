using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }

public class ChildDictionary : MonoBehaviour
{
  public GameObject pivot;
  public GameObject offset;
  public GameObject model;
  public GameObject vrTrackingData;

  public StringGameObjectDictionary modelChildren;
  public StringGameObjectDictionary vrTrackingDataChildren;

  void Awake()
  {
    modelChildren = new StringGameObjectDictionary();
    vrTrackingDataChildren = new StringGameObjectDictionary();

    modelChildren.CopyFrom(model.transform.AllChildren()
      .GroupBy(transform => transform.name)
      .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));

    vrTrackingDataChildren.CopyFrom(vrTrackingData.transform.AllChildren()
      .GroupBy(transform => transform.name)
      .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));
  }
}
