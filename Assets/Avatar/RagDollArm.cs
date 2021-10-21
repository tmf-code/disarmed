using UnityEngine;

class RagDollArm : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var wrist = transform.FindRecursiveOrThrow($"b_{handPrefix}_wrist").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;

    var forearmJoint = forearm.AddIfNotExisting<CharacterJoint>();
    forearmJoint.connectedBody = wrist.GetComponent<Rigidbody>();
    var humerusJoint = humerus.AddIfNotExisting<CharacterJoint>();
    humerusJoint.connectedBody = forearm.GetComponent<Rigidbody>();

    wrist.GetComponent<Rigidbody>().isKinematic = false;
    wrist.GetComponent<Rigidbody>().useGravity = true;
    wrist.GetComponent<CapsuleCollider>().isTrigger = false;

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
    var wrist = transform.FindRecursiveOrThrow($"b_{handPrefix}_wrist").gameObject;
    wrist.GetComponentOrThrow<Rigidbody>().isKinematic = true;
    wrist.GetComponentOrThrow<Rigidbody>().useGravity = false;
    wrist.GetComponent<CapsuleCollider>().isTrigger = true;

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