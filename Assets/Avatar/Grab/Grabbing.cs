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

    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 1;

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

    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;

    gameObject.AddIfNotExisting<Idle>();
    grabbed.SendMessage("OnGrabReleased", SendMessageOptions.RequireReceiver);

    Destroy(this);
  }
}

