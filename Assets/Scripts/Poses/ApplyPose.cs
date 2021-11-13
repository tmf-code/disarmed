using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Applies a saved pose to the model
/// Needs optimising (pairs)
/// </summary>
public class ApplyPose : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 0.0F;
  public Pose[] poses = new Pose[] { };
  public Option<Pose> pose = new None<Pose>();

  public string posePath = "Poses";
  private GameObject[] modelAndChildren;

  public void Start()
  {
    Load();

    var childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var modelChildren = childDictionary.modelChildren;
    var model = childDictionary.model;
    modelAndChildren = modelChildren.Values.ToArray();
    modelAndChildren.Append(model.gameObject);
  }

  public void Update()
  {
    if (strength <= 0F) return;
    if (pose.IsNone()) return;

    foreach (var current in modelAndChildren)
    {
      if (!BoneNameOperations.IsTrackedBone(current.name)) continue;
      if (!pose.Unwrap().transforms.TryGetValue(current.name, out var target)) continue;
      current.transform.LerpLocal(target.unSerialized, strength);
    }
  }

  public void Load()
  {
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
    poses = new Pose[] { };
    var textAssets = Resources.LoadAll(posePath);
    foreach (var item in textAssets)
    {
      if (item.GetType() == typeof(TextAsset))
      {
        TextAsset textAsset = item as TextAsset;
        var serializedTransforms = JsonUtility.FromJson<SerializedTransforms>(textAsset.text);
        var pose = new Pose(item.name, serializedTransforms);
        poses = poses.Append(pose).ToArray();
      }
    }
  }
}
