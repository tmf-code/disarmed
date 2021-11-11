using System.Linq;
using UnityEngine;

/// <summary>
/// Applies VRTrackingData Hand components to Model
/// </summary>
public class ApplyHandTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  [SerializeField]
  private TransformPair[] trackingDataChildrenPairs;

  void Start()
  {
    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var modelChildren = childDictionary.modelChildren;
    trackingDataChildrenPairs = childDictionary.vrTrackingDataChildren.Values.Where(child =>
    {
      return BoneNameOperations.IsTrackedBone(child.name) && child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
    }).Select(gameObject => new TransformPair(gameObject.transform, modelChildren.GetValue(gameObject.name).Unwrap().transform)).ToArray();
  }

  void Update()
  {
    foreach (var trackingAndModel in trackingDataChildrenPairs)
    {
      var tracking = trackingAndModel.Item1;
      var model = trackingAndModel.Item2;
      model.LerpLocal(tracking, strength);
    }
  }
}
