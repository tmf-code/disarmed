using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildDictionary : MonoBehaviour
{
  public GameObject pivot;
  public GameObject offset;
  public GameObject model;
  public GameObject vrTrackingData;

  public Dictionary<string, GameObject> modelChildren;
  public Dictionary<string, GameObject> vrTrackingDataChildren;

  void Awake()
  {
    modelChildren = model.transform.AllChildren()
      .GroupBy(transform => transform.name)
      .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject);

    vrTrackingDataChildren = vrTrackingData.transform.AllChildren()
      .GroupBy(transform => transform.name)
      .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject);
  }
}
