using System;
using UnityEngine;

public class BoneCollider : MonoBehaviour
{
  public enum CapsuleColliderDirection
  {
    X_AXIS = 0,
    Y_AXIS = 1,
    Z_AXIS = 2,
  }
  public BoneName boneStart;
  public BoneName boneEnd;

  private Transform boneStartTransform;
  private Transform boneEndTransform;

  public float radius = 0.1F;

  public GameObject colliderRoot;
  public new CapsuleCollider collider;

  public void Start()
  {
    colliderRoot = colliderRoot == null ? new GameObject($"BoneCollider_{boneStart}_TO_{boneEnd}") : colliderRoot;
    collider =
        colliderRoot.GetComponent<CapsuleCollider>() == null
            ? colliderRoot.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider
          : collider;

    collider.direction = (int)CapsuleColliderDirection.Y_AXIS;

    boneStartTransform = transform.FindRecursiveOrThrow(boneStart.ToString());
    boneEndTransform = transform.FindRecursiveOrThrow(boneEnd.ToString());
  }

  void Update()
  {

    var center = (boneStartTransform.position + boneEndTransform.position) / 2;
    var start = boneStartTransform.position;
    var end = boneEndTransform.position;

    var distance = end - start;
    var direction = distance.normalized;
    var startAxis = new Vector3(0F, 1F, 0F);

    var rotation = Quaternion.FromToRotation(startAxis, direction);
    colliderRoot.transform.SetPositionAndRotation(center, rotation);

    collider.radius = radius;
    collider.height = distance.magnitude + radius * 2;
  }
}

public static class ExtensionMethods
{
  public static Transform FindRecursiveOrThrow(this Transform transform, string childName)
  {
    var maybeChild = transform.FindChildRecursive(childName);

    if (maybeChild == null)
    {
      throw new Exception($"Could not find child: {childName}");
    }

    return maybeChild;
  }
}