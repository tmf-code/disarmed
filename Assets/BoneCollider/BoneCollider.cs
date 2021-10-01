using UnityEngine;

public class BoneCollider : MonoBehaviour
{
  public enum CapsuleColliderDirection
  {
    X_AXIS = 0,
    Y_AXIS = 1,
    Z_AXIS = 2,
  }
  public Transform boneStart;
  public Transform boneEnd;
  public float radius = 0.1F;

  public GameObject colliderRoot;
  new public CapsuleCollider collider;
  // Start is called before the first frame update
  void Start()
  {
    colliderRoot = colliderRoot == null ? new GameObject("BoneCollider") : colliderRoot;
    collider =
        colliderRoot.GetComponent<CapsuleCollider>() == null
            ? colliderRoot.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider
            : collider;

    collider.direction = (int)CapsuleColliderDirection.Y_AXIS;
  }

  // Update is called once per frame
  void Update()
  {
    var center = (boneStart.position + boneEnd.position) / 2;
    var start = boneStart.position;
    var end = boneEnd.position;

    var distance = end - start;
    var direction = distance.normalized;
    var startAxis = new Vector3(0F, 1F, 0F);

    var rotation = Quaternion.FromToRotation(startAxis, direction);
    colliderRoot.transform.SetPositionAndRotation(center, rotation);

    collider.radius = radius;
    collider.height = distance.magnitude + radius * 2;
  }
}
