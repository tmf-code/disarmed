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
  public new Rigidbody rigidbody;
  public SendCollisionToTarget eventSender;

  public void Start()
  {
    colliderRoot = colliderRoot == null ? new GameObject($"BoneCollider_{boneStart}_TO_{boneEnd}") : colliderRoot;
    colliderRoot.transform.parent = transform;
    collider =
        colliderRoot.GetComponent<CapsuleCollider>() == null
            ? colliderRoot.AddComponent<CapsuleCollider>()
          : collider;

    collider.direction = (int)CapsuleColliderDirection.Y_AXIS;
    collider.isTrigger = true;

    rigidbody =
        colliderRoot.GetComponent<Rigidbody>() == null
            ? colliderRoot.AddComponent<Rigidbody>()
          : rigidbody;

    rigidbody.useGravity = false;
    rigidbody.isKinematic = true;

    eventSender =
        colliderRoot.GetComponent<SendCollisionToTarget>() == null
            ? colliderRoot.AddComponent<SendCollisionToTarget>()
          : eventSender;
    eventSender.target = gameObject;

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
    rigidbody.position = center;
    rigidbody.rotation = rotation;

    collider.radius = radius;
    collider.height = distance.magnitude + radius * 2;
  }
}
