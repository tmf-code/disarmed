using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }
[Serializable]
public class GameObjectGameObjectDictionary : SerializableDictionary<GameObject, GameObject> { }

public class ChildDictionary : MonoBehaviour
{
  public GameObject pivot;
  public GameObject offset;
  public GameObject model;
  public GameObject vrTrackingData;

  public StringGameObjectDictionary modelChildren;
  public StringGameObjectDictionary vrTrackingDataChildren;

  [HideInInspector]
  public TransformPair[] handBonePairs;
  [HideInInspector]
  public GameObjectGameObjectDictionary trackingToModel;

  void Awake()
  {

    handBonePairs = vrTrackingDataChildren.Values.Where(child =>
    {
      var isTrackedBone = BoneNameOperations.IsTrackedBone(child.name);
      var isNotIKBone = child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
      var isHandBone = isTrackedBone && isNotIKBone;
      return isHandBone;
    }).Select(bone =>
    {
      var modelBoneTransform = modelChildren.GetValue(bone.name).Unwrap().transform;
      return new TransformPair(bone.transform, modelBoneTransform);
    }).ToArray();

    trackingToModel = new GameObjectGameObjectDictionary();
    trackingToModel.CopyFrom(vrTrackingDataChildren.Select(pair =>
    {
      return new { tracking = pair.Value, model = modelChildren.GetValue(pair.Key).Unwrap() };
    }).ToDictionary(pair => pair.tracking, pair => pair.model));

    // JP - Left here in case the model hierarchy changes. Use this then you can copy paste the values
    // From runtime into editor time
    // modelChildren = new StringGameObjectDictionary();
    // vrTrackingDataChildren = new StringGameObjectDictionary();

    // modelChildren.CopyFrom(model.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));

    // vrTrackingDataChildren.CopyFrom(vrTrackingData.transform.AllChildren()
    //   .GroupBy(transform => transform.name)
    //   .ToDictionary(transforms => transforms.Key, transforms => transforms.First().gameObject));
  }
}
