using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyHandTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private GameObject[] trackingDataChildren;
  private Dictionary<string, GameObject> modelChildren;
  private ChildDictionary childDictionary;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    trackingDataChildren = childDictionary.vrTrackingDataChildren.Values.Where(child =>
    {
      return BoneNameToBoneId.IsTrackedBone(child.name) && child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
    }).ToArray();
    modelChildren = childDictionary.modelChildren;
  }

  void Update()
  {
    foreach (var target in trackingDataChildren)
    {
      var current = modelChildren.GetValue(target.name).transform;
      current.LerpLocal(target.transform, strength);
    }
  }
}
