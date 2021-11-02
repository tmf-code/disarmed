using UnityEngine;

public class PivotPoint : MonoBehaviour
{
  public PivotPointType pivotPointType = PivotPointType.Wrist;

  public enum PivotPointType
  {
    Wrist,
    Shoulder,
  }

  void Update()
  {
    var offset = transform.FindRecursiveOrThrow("Offset");
    var pivot = transform.FindRecursiveOrThrow("Pivot");

    if (pivotPointType == PivotPointType.Wrist)
    {
      offset.localPosition = Vector3.zero;
    }
    else
    {
      var handedness = gameObject.GetComponentOrThrow<Handedness>();
      var handPrefix = handedness.HandPrefix();

      var model = transform.FindRecursiveOrThrow("Model");
      var shoulder = model.FindRecursiveOrThrow($"b_{handPrefix}_shoulder");

      var distance = offset.position - shoulder.position;

      offset.localPosition = Quaternion.Inverse(offset.rotation) * distance;

      offset.localPosition = new Vector3(
          offset.localPosition.x / offset.lossyScale.x,
          offset.localPosition.y / offset.lossyScale.y,
          offset.localPosition.z / offset.lossyScale.z);

      var rotation = Quaternion.Inverse(shoulder.rotation) * model.rotation;
      pivot.localRotation = rotation;
    }
  }
}
