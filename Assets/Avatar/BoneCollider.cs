using UnityEngine;

public class BoneCollider : MonoBehaviour
{
  public Transform boneStart;
  public Transform boneEnd;

  public GameObject colliderRoot;
  new public CapsuleCollider collider;
  // Start is called before the first frame update
  void Start()
  {
    colliderRoot = colliderRoot == null ? new GameObject("BoneCollider") : colliderRoot;
    collider =
        colliderRoot.GetComponent<CapsuleCollider>() == null
            ? gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider
            : collider;
  }

  // Update is called once per frame
  void Update()
  {
    var center = (boneStart.position + boneEnd.position) / 2;
  }
}
