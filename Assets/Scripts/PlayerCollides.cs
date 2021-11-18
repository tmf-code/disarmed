using UnityEngine;

class PlayerCollides : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var model = transform.FindRecursiveOrThrow($"Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    model.GetComponent<CapsuleCollider>().isTrigger = false;
    forearm.GetComponent<CapsuleCollider>().isTrigger = false;
    humerus.GetComponent<CapsuleCollider>().isTrigger = false;
  }
}