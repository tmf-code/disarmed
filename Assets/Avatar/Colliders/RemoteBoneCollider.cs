using UnityEngine;

public class RemoteBoneCollider : MonoBehaviour
{
  public new string tag;
  public BoneName boneStart;
  public BoneName boneEnd;

  private Transform boneStartTransform;
  private Transform boneEndTransform;

  private readonly float radius = 0.05F;

  public GameObject colliderRoot;
  public new CapsuleCollider collider;
  public new Rigidbody rigidbody;
  public SendCollisionToTarget eventSender;

  public void Start()
  {
    colliderRoot = colliderRoot == null ? new GameObject($"BoneCollider_{boneStart}_TO_{boneEnd}") : colliderRoot;
    colliderRoot.transform.parent = transform;

    collider = colliderRoot.AddIfNotExisting<CapsuleCollider>();
    collider.direction = (int)CapsuleColliderDirection.Y_AXIS;
    collider.isTrigger = true;
    collider.tag = tag;
    collider.radius = radius;

    rigidbody = colliderRoot.AddIfNotExisting<Rigidbody>();

    rigidbody.useGravity = false;
    rigidbody.isKinematic = true;

    eventSender = colliderRoot.AddIfNotExisting<SendCollisionToTarget>();

    eventSender.target = gameObject;
    eventSender.source = collider;

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

  void OnDestroy()
  {
    Destroy(colliderRoot);
  }
}
