using UnityEngine;

public class PivotPoint : MonoBehaviour
{

  public PivotPointType pivotPointType = PivotPointType.Wrist;
  public ChildDictionary childDictionary;

  public Handedness handedness;

  [Button(nameof(LateUpdate))]
  public bool buttonField;

  public enum PivotPointType
  {
    Wrist,
    Elbow,
    Shoulder,
    ShoulderNoRotation,
    None,
  }

  public void LateUpdate()
  {
    var offset = childDictionary.offset;
    var model = childDictionary.model;
    var pivot = childDictionary.pivot;
    var elbow = childDictionary.modelHumerus;
    var shoulder = childDictionary.modelShoulder;

    if (pivotPointType == PivotPointType.None) return;

    if (pivotPointType == PivotPointType.Wrist)
    {
      offset.localPosition = Vector3.zero;
      return;
    }

    if (pivotPointType == PivotPointType.Elbow)
    {
      var distance2 = offset.position - elbow.position;
      offset.localPosition = Quaternion.Inverse(offset.rotation) * distance2;
      offset.localPosition = new Vector3(
          offset.localPosition.x / offset.lossyScale.x,
          offset.localPosition.y / offset.lossyScale.y,
          offset.localPosition.z / offset.lossyScale.z);
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
