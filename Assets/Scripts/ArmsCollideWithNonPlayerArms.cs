using UnityEngine;

class ArmsCollideWithNonPlayerArms : MonoBehaviour
{
  void Start()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    var model = transform.FindRecursiveOrThrow($"Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;

    var modelRigidBody = model.GetComponent<Rigidbody>();
    var forearmRigidBody = forearm.GetComponent<Rigidbody>();
    var humerusRigidbody = humerus.GetComponent<Rigidbody>();


    var forearmJoint = forearm.AddIfNotExisting<ConfigurableJoint>();
    SetupAsCharacterJoint(forearmJoint);
    forearmJoint.connectedBody = modelRigidBody;
    var humerusJoint = humerus.AddIfNotExisting<ConfigurableJoint>();
    SetupAsCharacterJoint(humerusJoint);
    humerusJoint.connectedBody = forearmRigidBody;

    modelRigidBody.isKinematic = false;
    modelRigidBody.useGravity = true;
    model.GetComponent<CapsuleCollider>().isTrigger = false;


    forearmRigidBody.isKinematic = false;
    forearmRigidBody.useGravity = true;
    forearm.GetComponent<CapsuleCollider>().isTrigger = false;

    humerusRigidbody.isKinematic = false;
    humerusRigidbody.useGravity = true;
    humerus.GetComponent<CapsuleCollider>().isTrigger = false;
  }

  void OnDestroy()
  {
    var handPrefix = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();
    var model = transform.FindRecursiveOrThrow("Model").gameObject;
    var forearm = transform.FindRecursiveOrThrow($"b_{handPrefix}_forearm_stub").gameObject;
    var humerus = transform.FindRecursiveOrThrow($"b_{handPrefix}_humerus").gameObject;

    var modelRigidBody = model.GetComponent<Rigidbody>();
    var forearmRigidBody = forearm.GetComponent<Rigidbody>();
    var humerusRigidbody = humerus.GetComponent<Rigidbody>();

    modelRigidBody.isKinematic = true;
    modelRigidBody.useGravity = false;
    model.GetComponent<CapsuleCollider>().isTrigger = true;

    forearm.RemoveComponent<ConfigurableJoint>();
    forearmRigidBody.isKinematic = true;
    forearmRigidBody.useGravity = false;
    forearmRigidBody.detectCollisions = false;
    forearm.GetComponent<CapsuleCollider>().isTrigger = true;

    humerus.RemoveComponent<ConfigurableJoint>();
    humerusRigidbody.isKinematic = true;
    humerusRigidbody.useGravity = false;
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