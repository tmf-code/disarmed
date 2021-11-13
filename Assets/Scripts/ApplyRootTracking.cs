using UnityEngine;

/// <summary>
/// Applies the transform of hand tracking to model
/// </summary>
public class ApplyRootTracking : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private Transform trackingData;
  private Transform model;

  void Start()
  {
    model = gameObject.GetComponentOrThrow<ChildDictionary>().model;
    trackingData = gameObject.GetComponentOrThrow<DataSources>().trackingHandRootData.handRoot;
  }

  void Update()
  {
    model.LerpLocal(trackingData, strength);
  }
}
