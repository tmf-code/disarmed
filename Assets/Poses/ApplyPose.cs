using System.Linq;
using UnityEditor;
using UnityEngine;

public class ApplyPose : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 0.0F;
  public Pose[] poses = new Pose[] { };
  [SerializeField]
  private Transform model;

  public string posePath = "Poses";

  public void Start()
  {
    Load();
    model = transform.FindRecursiveOrThrow("Model");
  }

  public void Update()
  {
    void ApplyTransform(Transform current)
    {
      if (!BoneNameToBoneId.IsTrackedBone(current.name)) return;
      if (poses.Length == 0)
      {
        return;
      }
      var maybePose = poses.First();
      if (maybePose == null) return;
      // Ideally Pose.transforms should be serialized (ie: not a dictionary) so that we can ignore the following check
      if (maybePose.transforms == null) return;

      if (!maybePose.transforms.TryGetValue(current.name, out var target)) return;

      current.localPosition = Vector3.Lerp(current.localPosition, target.unSerialized.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.unSerialized.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.unSerialized.localScale, strength);

    }
    ApplyTransform(model);
    model.TraverseChildren(ApplyTransform);
  }

  public void Load()
  {
    AssetDatabase.Refresh();

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
