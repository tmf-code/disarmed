using UnityEngine;

public class Idle : MonoBehaviour
{
  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  private ArmBehavior armBehavior;

  void Start()
  {
    creationTime = Time.time;
    gameObject.GetOptionComponent<InverseKinematics>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);

    gameObject.RemoveComponent<Grabbed>();
    gameObject.RemoveComponent<Grabbing>();

    armBehavior = gameObject.GetComponentOrThrow<ArmBehavior>();

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

    var otherParent = colliders.other.transform.parent.gameObject;
    var isCollidingWithSelf = otherParent == gameObject;
    if (isCollidingWithSelf) { return; }

    if (otherParent.HasComponent<Grabbed>() || otherParent.HasComponent<Grabbing>())
    {
      return;
    }

    var source = colliders.source;
    var other = colliders.other;

    var sourceIsUserArm = armBehavior.behavior == ArmBehavior.ArmBehaviorType.User;
    var shouldGrab = source.CompareTag("Hand") && other.CompareTag("Forearm") && sourceIsUserArm;
    if (shouldGrab)
    {
      var grabbing = gameObject.AddIfNotExisting<Grabbing>();
      var grabbed = otherParent.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }

    var otherIsUserArm = otherParent.GetComponentOrThrow<ArmBehavior>().behavior == ArmBehavior.ArmBehaviorType.User;
    var shouldBeGrabbed = source.CompareTag("Forearm") && other.CompareTag("Hand") && !otherIsUserArm;
    if (shouldBeGrabbed)
    {
      var grabbing = otherParent.AddIfNotExisting<Grabbing>();
      var grabbed = gameObject.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }
  }
}