using System;
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
  [HideInInspector]
  private LocalRotationTransformPair[] handBonePairs;

  void Start()
  {
    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var data = gameObject.GetComponentOrThrow<DataSources>().trackingHandBoneData.bones;
    handBonePairs = data.Select(nameGameObjectKV =>
    {
      var source = nameGameObjectKV.Value;
      var name = nameGameObjectKV.Key;
      var destination = childDictionary.modelChildren.GetValue(name).Unwrap().transform;
      return new LocalRotationTransformPair(source, destination);
    }).ToArray();
  }

  void Update()
  {
    foreach (var trackingAndModel in handBonePairs)
    {
      var rotation = trackingAndModel.rotation.localRotation;
      var transform = trackingAndModel.transform;
      transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, rotation, strength);
    }
  }
}

[Serializable]
public struct LocalRotationTransformPair
{
  public LocalRotation rotation;
  public Transform transform;

  public LocalRotationTransformPair(LocalRotation rotation, Transform transform)
  {
    this.rotation = rotation;
    this.transform = transform;
  }
}
