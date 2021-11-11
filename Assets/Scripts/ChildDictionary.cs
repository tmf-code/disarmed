using System;
using UnityEngine;

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }
[Serializable]
public class GameObjectGameObjectDictionary : SerializableDictionary<GameObject, GameObject> { }

public class ChildDictionary : MonoBehaviour
{
  public GameObject pivot;
  public GameObject offset;
  public GameObject model;
  public GameObject vrTrackingData;

  public StringGameObjectDictionary modelChildren;
  public StringGameObjectDictionary vrTrackingDataChildren;

  public GameObjectGameObjectDictionary trackingToModel;

  void Awake()
  {
    // modelChildren = new StringGameObjectDictionary();
    // vrTrackingDataChildren = new StringGameObjectDictionary();

    // modelChildren.CopyFrom(model.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));

    // vrTrackingDataChildren.CopyFrom(vrTrackingData.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));
  }
}
