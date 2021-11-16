using UnityEngine;

public class Grabbing : MonoBehaviour
{
  public Grabbed grabbed;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  public bool fakeHandOpen = false;
  private GestureData gestureData;

  void Start()
  {
    creationTime = Time.time;
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 1);

    gestureData = gameObject.GetComponent<DataSources>().gestureData;

    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbed>();
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

    if (gestureData.handOpen)
    {
      OnHandOpen();
    }
  }

  public void OnHandOpen()
  {
    if (!canTransition) return;
    Option<Grabbed>.of(grabbed).End(grabbed => grabbed.OnGrabReleased());
    Destroy(this);
  }

  public void OnDestroy()
  {
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);
    gameObject.AddIfNotExisting<Idle>();
    Option<Grabbed>.of(grabbed).End(grabbed => grabbed.OnGrabReleased());
  }

  void OnValidate()
  {
    if (fakeHandOpen)
    {
      OnHandOpen();
      fakeHandOpen = false;
    }
  }

  public void OnArmAttach()
  {
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);

    gameObject.AddIfNotExisting<Idle>();

    Destroy(this);
  }
}

