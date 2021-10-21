using UnityEngine;

class RagDollArm : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var wrist = transform.FindRecursiveOrThrow($"b_{handPrefix}_wrist").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    var shoulder = transform.FindRecursiveOrThrow($"b_{handPrefix}_shoulder").gameObject;

    var forearmJoint = forearm.AddIfNotExisting<CharacterJoint>();
    AddBoneCollider(wrist, forearm, 0.05F);
    wrist.AddIfNotExisting<Rigidbody>();

    AddBoneCollider(forearm, humerus, 0.05F);
    forearm.AddIfNotExisting<Rigidbody>();
    forearmJoint.connectedBody = wrist.GetComponent<Rigidbody>();

    AddBoneCollider(humerus, shoulder, 0.05F);
    humerus.AddIfNotExisting<Rigidbody>();
    var humerusJoint = humerus.AddIfNotExisting<CharacterJoint>();
    humerusJoint.connectedBody = forearm.GetComponent<Rigidbody>();
  }

  void OnDestroy()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var wrist = transform.FindRecursiveOrThrow($"b_{handPrefix}_wrist").gameObject;
    wrist.RemoveComponent<CapsuleCollider>();
    wrist.RemoveComponent<Rigidbody>();

    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    forearm.RemoveComponent<CapsuleCollider>();
    forearm.RemoveComponent<Rigidbody>();
    forearm.RemoveComponent<CharacterJoint>();

    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    humerus.RemoveComponent<CapsuleCollider>();
    humerus.RemoveComponent<Rigidbody>();
    humerus.RemoveComponent<CharacterJoint>();
  }

  public static CapsuleCollider AddBoneCollider(GameObject rootBone, GameObject childBone, float radius)
  {
    var result = rootBone.AddComponent<CapsuleCollider>();
    result.direction = (int)CapsuleColliderDirection.Y_AXIS;

    var end = childBone.transform.localPosition;
    result.radius = radius;
    result.height = end.magnitude;
    result.center = new Vector3(-end.magnitude / 2F, 0, 0);

    return result;
  }
}