using UnityEngine;

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

    var grabbing = gameObject.AddIfNotExisting<Grabbing>();
    var grabbed = toBeGrabbed.gameObject.AddIfNotExisting<Grabbed>();

    grabbed.gameObject.GetComponentOrThrow<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
    grabbing.grabbed = grabbed;
    grabbed.grabbing = grabbing;

    Destroy(this);
    Destroy(toBeGrabbed);
  }
}