using UnityEngine;

public class Grabbing : MonoBehaviour
{
  public Grabbed grabbed;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  public bool fakeHandOpen = false;

  void Start()
  {
    creationTime = Time.time;
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 1);

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
  }

  public void OnHandOpen()
  {
    if (!canTransition) return;
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);

    gameObject.AddIfNotExisting<Idle>();
    Option<Grabbed>.of(grabbed).End(grabbed => SendMessage("OnGrabReleased", SendMessageOptions.DontRequireReceiver));

    Destroy(this);
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

