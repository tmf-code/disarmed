using UnityEngine;

class PlayerInteracts : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var model = transform.FindRecursiveOrThrow($"Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    model.GetComponent<CapsuleCollider>().isTrigger = true;
    forearm.GetComponent<CapsuleCollider>().isTrigger = true;
    humerus.GetComponent<CapsuleCollider>().isTrigger = true;
  }
}