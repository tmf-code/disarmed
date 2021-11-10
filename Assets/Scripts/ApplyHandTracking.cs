using System.Linq;
using UnityEngine;

public class ApplyHandTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private GameObject[] trackingDataChildren;
  private ChildDictionary childDictionary;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    trackingDataChildren = childDictionary.vrTrackingDataChildren.Values.Where(child =>
    {
      return BoneNameOperations.IsTrackedBone(child.name) && child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
    }).ToArray();
  }

  void Update()
  {

    foreach (var target in trackingDataChildren)
    {
      var current = childDictionary.modelChildren.GetValue(target.name).Unwrap().transform;
      current.LerpLocal(target.transform, strength);
    }
  }
}
