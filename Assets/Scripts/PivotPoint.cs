using UnityEngine;

public class PivotPoint : MonoBehaviour
{
  public PivotPointType pivotPointType = PivotPointType.Wrist;
  private Transform offset;
  private Transform pivot;
  private Transform model;
  private Transform shoulder;
  private Handedness handedness;

  public enum PivotPointType
  {
    Wrist,
    Shoulder,
    ShoulderNoRotation,
  }

  void Start()
  {
    handedness = gameObject.GetComponentOrThrow<Handedness>();
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
      pivot.rotation = rotation;

      pivot.Rotate(new Vector3(0, 0, 90), Space.World);

      if (handedness.IsLeft())
      {
        pivot.Rotate(new Vector3(180, 0, 0), Space.World);
      }
      else
      {
        pivot.Rotate(new Vector3(0, 180, 0), Space.World);
      }
    }

  }
}
