using UnityEngine;

class RagDollArm : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var model = transform.FindRecursiveOrThrow($"Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;

    var forearmJoint = forearm.AddIfNotExisting<ConfigurableJoint>();
    SetupAsCharacterJoint(forearmJoint);

    forearmJoint.connectedBody = model.GetComponent<Rigidbody>();

    var humerusJoint = humerus.AddIfNotExisting<ConfigurableJoint>();
    SetupAsCharacterJoint(humerusJoint);
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
    forearm.RemoveComponent<ConfigurableJoint>();
    forearm.GetComponent<Rigidbody>().isKinematic = true;
    forearm.GetComponent<Rigidbody>().useGravity = false;
    forearm.GetComponent<CapsuleCollider>().isTrigger = true;

    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;
    humerus.RemoveComponent<ConfigurableJoint>();
    humerus.GetComponent<Rigidbody>().isKinematic = true;
    humerus.GetComponent<Rigidbody>().useGravity = false;
    humerus.GetComponent<CapsuleCollider>().isTrigger = true;
  }

  /// <summary>
  /// https://gist.github.com/mstevenson/7b85893e8caf5ca034e6
  /// </summary>
  public static void SetupAsCharacterJoint(ConfigurableJoint joint)
  {
    joint.xMotion = ConfigurableJointMotion.Locked;
    joint.yMotion = ConfigurableJointMotion.Locked;
    joint.zMotion = ConfigurableJointMotion.Locked;
    joint.angularXMotion = ConfigurableJointMotion.Limited;
    joint.angularYMotion = ConfigurableJointMotion.Limited;
    joint.angularZMotion = ConfigurableJointMotion.Limited;

    var jd = new JointDrive();
    jd.positionSpring = 1500;
    jd.positionDamper = 10;
    joint.angularXDrive = jd;
    joint.angularYZDrive = jd;

    joint.breakForce = Mathf.Infinity;
    joint.breakTorque = Mathf.Infinity;
    joint.configuredInWorldSpace = false;


    var yLimit = joint.angularYLimit;
    var zLimit = joint.angularZLimit;

    if (joint.name.Contains("forearm"))
    {
      yLimit.limit = 177;
      zLimit.limit = 30;
    }
    else
    {
      zLimit.limit = 177;
      yLimit.limit = 30;
    }

    joint.angularYLimit = yLimit;
    joint.angularZLimit = zLimit;

    joint.rotationDriveMode = RotationDriveMode.Slerp;
    var slerpDrive = joint.slerpDrive;
    slerpDrive.mode = JointDriveMode.Position;
    slerpDrive.maximumForce = Mathf.Infinity;
    slerpDrive.positionSpring = 1500;
    slerpDrive.positionDamper = 10;
    joint.slerpDrive = slerpDrive;
  }
}