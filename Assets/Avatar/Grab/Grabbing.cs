using UnityEngine;

public class Grabbing : MonoBehaviour
{
  public Grabbed grabbed;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  void Start()
  {
    creationTime = Time.time;

    transform.GetComponent<InverseKinematics>().enabled = true;
    var skeleton = transform.GetComponent<CustomSkeleton>();
    skeleton.enabled = true;

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

    var skeleton = transform.GetComponent<CustomSkeleton>();


    gameObject.AddIfNotExisting<Idle>();
    grabbed.SendMessage("OnGrabReleased", SendMessageOptions.RequireReceiver);

    Destroy(this);
  }
}

