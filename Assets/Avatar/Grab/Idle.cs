using UnityEngine;

public class Idle : MonoBehaviour
{
  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  private ArmBehaviour armBehavior;

  void Start()
  {
    creationTime = Time.time;
    gameObject.GetOptionComponent<InverseKinematics>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);

    gameObject.RemoveComponent<Grabbed>();
    gameObject.RemoveComponent<Grabbing>();

    armBehavior = gameObject.GetComponentOrThrow<ArmBehaviour>();
  }

  void Update()
  {
    var currentTime = Time.time - creationTime;
    if (currentTime > minimumIdleTimeSeconds)
    {
      canTransition = true;
    }
    else
    {
      canTransition = false;
    }
  }

  public void OnTriggersEnter(TwoPartyCollider colliders)
  {
    if (!canTransition) return;
    var isSourceAnArm = colliders.source.CompareTag("Hand") || colliders.source.CompareTag("Forearm");
    var isOtherAnArm = colliders.other.CompareTag("Hand") || colliders.other.CompareTag("Forearm");
    if (!isSourceAnArm || !isOtherAnArm)
    {
      // Is some other object in the scene that we don't care about here
      return;
    }

    var isSameObject = colliders.otherTarget == colliders.sourceTarget;
    if (isSameObject)
    {
      return;
    }


    if (colliders.otherTarget.HasComponent<Grabbed>() || colliders.otherTarget.HasComponent<Grabbing>())
    {
      return;
    }

    var bothAreArms = gameObject.HasComponent<ArmBehaviour>() && colliders.otherTarget.HasComponent<ArmBehaviour>();
    if (!bothAreArms)
    {
      return;
    }

    var neitherAreTypeNone =
      gameObject.GetComponent<ArmBehaviour>().behavior != ArmBehaviour.ArmBehaviorType.None
      && colliders.otherTarget.GetComponent<ArmBehaviour>().behavior != ArmBehaviour.ArmBehaviorType.None;

    if (!neitherAreTypeNone) return;

    var source = colliders.source;
    var other = colliders.other;

    var sourceIsUserArm = armBehavior.owner == ArmBehaviour.ArmOwnerType.User;
    var shouldGrab = source.CompareTag("Hand") && other.CompareTag("Forearm") && sourceIsUserArm;
    if (shouldGrab)
    {
      var grabbing = gameObject.AddIfNotExisting<Grabbing>();
      var grabbed = colliders.otherTarget.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }

    var otherIsUserArm = colliders.otherTarget.GetComponentOrThrow<ArmBehaviour>().owner == ArmBehaviour.ArmOwnerType.User;
    var shouldBeGrabbed = source.CompareTag("Forearm") && other.CompareTag("Hand") && otherIsUserArm;
    if (shouldBeGrabbed)
    {
      var grabbing = colliders.otherTarget.AddIfNotExisting<Grabbing>();
      var grabbed = gameObject.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }
  }
}