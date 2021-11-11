using UnityEngine;

/// <summary>
/// Applies the transform of VRTrackingData to Model
/// </summary>
public class ApplyRootTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private Transform trackingData;
  private Transform model;

  void Start()
  {
    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    model.LerpLocal(trackingData, strength);
  }
}
