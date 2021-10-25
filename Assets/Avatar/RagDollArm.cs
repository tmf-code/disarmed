using UnityEngine;

class RagDollArm : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var model = transform.FindRecursiveOrThrow($"Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;

    var forearmJoint = forearm.AddIfNotExisting<CharacterJoint>();
    forearmJoint.connectedBody = model.GetComponent<Rigidbody>();
    var humerusJoint = humerus.AddIfNotExisting<CharacterJoint>();
    humerusJoint.connectedBody = forearm.GetComponent<Rigidbody>();

    model.GetComponent<Rigidbody>().isKinematic = false;
    model.GetComponent<Rigidbody>().useGravity = true;
    model.GetComponent<CapsuleCollider>().isTrigger = false;

    forearm.GetComponent<Rigidbody>().isKinematic = false;
    forearm.GetComponent<Rigidbody>().useGravity = true;
    forearm.GetComponent<CapsuleCollider>().isTrigger = false;

    humerus.GetComponent<Rigidbody>().isKinematic = false;
    humerus.GetComponent<Rigidbody>().useGravity = true;
    humerus.GetComponent<CapsuleCollider>().isTrigger = false;
  }

  void OnDestroy()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();
    var model = transform.FindRecursiveOrThrow("Model").gameObject;
    model.GetComponentOrThrow<Rigidbody>().isKinematic = true;
    model.GetComponentOrThrow<Rigidbody>().useGravity = false;
    model.GetComponent<CapsuleCollider>().isTrigger = true;

    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    forearm.RemoveComponent<CharacterJoint>();
    forearm.GetComponent<Rigidbody>().isKinematic = true;
    forearm.GetComponent<Rigidbody>().useGravity = false;
    forearm.GetComponent<CapsuleCollider>().isTrigger = true;

    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    humerus.RemoveComponent<CharacterJoint>();
    humerus.GetComponent<Rigidbody>().isKinematic = true;
    humerus.GetComponent<Rigidbody>().useGravity = false;
    humerus.GetComponent<CapsuleCollider>().isTrigger = true;
  }
}