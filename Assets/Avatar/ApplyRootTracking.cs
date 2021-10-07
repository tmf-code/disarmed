using UnityEngine;

public class ApplyRootTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 0.0F;
  private Transform trackingData;
  private Transform model;

  void Start()
  {
    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    model.localPosition = Vector3.Lerp(model.localPosition, trackingData.localPosition, strength);
    model.localRotation = Quaternion.Slerp(model.localRotation, trackingData.localRotation, strength);
    model.localScale = Vector3.Lerp(model.localScale, trackingData.localScale, strength);
  }
}
