using UnityEngine;

public class ApplyTracking : MonoBehaviour
{
  public bool apply = false;
  private Transform trackingData;
  private Transform model;

  void Start()
  {
    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model");
  }

  void Update()
  {
    if (!apply) return;
    void CopyTransform(Transform transform)
    {
      var destination = model.FindChildRecursive(transform.name);
      if (!destination) return;
      destination.localPosition = transform.localPosition;
      destination.localRotation = transform.localRotation;
      destination.localScale = transform.localScale;
    }

    model.localPosition = trackingData.localPosition;
    model.localRotation = trackingData.localRotation;
    model.localScale = trackingData.localScale;
    trackingData.TraverseChildren(CopyTransform);
  }
}
