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
    trackingDataChildren = childDictionary.vrTrackingDataChildren.Values.ToArray();
    modelChildren = childDictionary.modelChildren;
  }

  void Update()
  {
    foreach (var target in trackingDataChildren)
    {
      if (!BoneNameToBoneId.IsTrackedBone(target.name)) continue;

      if (target.name == "b_l_forearm_stub" || target.name == "b_r_forearm_stub")
      {
        // Bone shouldn't be controlled by tracking. Conflicts with IK
        continue;
      }
      var current = modelChildren.GetValue(target.name).transform;

      current.LerpLocal(target.transform, strength);
    }
  }
}
