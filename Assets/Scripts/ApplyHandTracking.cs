using UnityEngine;

/// <summary>
/// Applies VRTrackingData Hand components to Model
/// </summary>
public class ApplyHandTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;

  void Start()
  {
    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    handBonePairs = childDictionary.handBonePairs;
  }

  void Update()
  {
    foreach (var trackingAndModel in handBonePairs)
    {
      var tracking = trackingAndModel.Item1;
      var model = trackingAndModel.Item2;
      model.LerpLocal(tracking, strength);
    }
  }
}
