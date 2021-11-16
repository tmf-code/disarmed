using UnityEngine;
using static WorldArmBehaviour;

public class Idle : MonoBehaviour
{
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  private float creationTime;

  void Start()
  {
    creationTime = Time.time;
    gameObject.RemoveComponent<Grabbed>();
    gameObject.RemoveComponent<Grabbing>();
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

  public void OnGrabBegin(Idle toBeGrabbed)
  {
    if (!toBeGrabbed.canTransition || !canTransition) return;

    if (toBeGrabbed.TryGetComponent<PlayerArmBehaviour>(out var playerArmBehaviour))
    {
      var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
      playerArms.RemoveArm(playerArmBehaviour, WorldArmBehaviours.Grabbed);
    }

    var grabbing = gameObject.AddIfNotExisting<Grabbing>();
    var grabbed = toBeGrabbed.gameObject.AddIfNotExisting<Grabbed>();

    var worldArmBehaviour = toBeGrabbed.gameObject.GetComponentOrThrow<WorldArmBehaviour>();
    worldArmBehaviour.behaviour = WorldArmBehaviours.Grabbed;

    var pivotPoint = grabbed.gameObject.GetComponentOrThrow<PivotPoint>();
    pivotPoint.pivotPointType = PivotPoint.PivotPointType.None;

    grabbing.grabbed = grabbed;
    grabbed.grabbing = grabbing;

    Destroy(this);
    Destroy(toBeGrabbed);
  }
}