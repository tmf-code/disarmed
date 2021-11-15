using System.Linq;
using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 30F / 60F;
  public GameObject targetArm;
  private DataSources dataSources;
  [SerializeField]
  [HideInInspector]

  private ChildDictionary childDictionary;
  private Transform forearm;
  private Transform humerus;
  private Transform model;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    dataSources = gameObject.GetComponentOrThrow<DataSources>();

    forearm = childDictionary.modelForearm;
    humerus = childDictionary.modelHumerus;
    model = childDictionary.model;
  }

  public int frameDelay = 0;

  void FixedUpdate()
  {
    var maybeFrame = dataSources.sharedFrameBuffer.frameQueue.ElementAtOrDefault(dataSources.sharedFrameBuffer.frameQueue.Count - frameDelay - 1);
    if (maybeFrame != null)
    {
      var frame = maybeFrame;

      forearm.localRotation = Quaternion.SlerpUnclamped(forearm.localRotation, frame.forearm, strength);
      humerus.localRotation = Quaternion.SlerpUnclamped(humerus.localRotation, frame.humerus, strength);
      model.localRotation = Quaternion.SlerpUnclamped(model.localRotation, frame.model, strength);

      foreach (var nameRotation in frame.trackingHandBoneData)
      {
        var source = nameRotation.Key;
        if (childDictionary.modelChildren.TryGetValue(source, out var destination))
        {
          destination.transform.localRotation = Quaternion.SlerpUnclamped(nameRotation.Value, destination.transform.localRotation, strength);
        }
      }
    }
  }
}
