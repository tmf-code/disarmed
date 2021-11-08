using UnityEngine;

public class PivotPoint : MonoBehaviour
{
  public PivotPointType pivotPointType = PivotPointType.Wrist;
  private Transform offset;
  private Transform pivot;
  private Transform model;
  private Transform shoulder;

  public enum PivotPointType
  {
    Wrist,
    Shoulder,
    ShoulderNoRotation,
  }

  void Start()
  {
    var handedness = gameObject.GetComponentOrThrow<Handedness>();
    var handPrefix = handedness.HandPrefix();
    offset = transform.FindRecursiveOrThrow("Offset");
    pivot = transform.FindRecursiveOrThrow("Pivot");
    model = transform.FindRecursiveOrThrow("Model");
    shoulder = model.FindRecursiveOrThrow($"b_{handPrefix}_shoulder");
  }

  void LateUpdate()
  {

    if (pivotPointType == PivotPointType.Wrist)
    {
      offset.localPosition = Vector3.zero;
      return;
    }

    var distance = offset.position - shoulder.position;
    offset.localPosition = Quaternion.Inverse(offset.rotation) * distance;
    offset.localPosition = new Vector3(
        offset.localPosition.x / offset.lossyScale.x,
        offset.localPosition.y / offset.lossyScale.y,
        offset.localPosition.z / offset.lossyScale.z);

    if (pivotPointType == PivotPointType.ShoulderNoRotation)
    {
      var rotation = Quaternion.Inverse(shoulder.rotation) * model.rotation;
      pivot.localRotation = rotation;
    }
  }
}
