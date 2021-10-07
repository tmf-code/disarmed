using UnityEngine;

public class Grabbed : MonoBehaviour
{
  public Grabbing grabbing;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = false;
    transform.GetComponent<CustomSkeleton>().enabled = false;

    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbing>();
  }

  void Update()
  {
    transform.SetPositionAndRotation(grabbing.transform.position, grabbing.transform.rotation);

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

  public void OnGrabReleased()
  {
    if (!canTransition) return;

    transform.GetComponent<InverseKinematics>().enabled = true;
    transform.GetComponent<CustomSkeleton>().enabled = true;

    gameObject.AddIfNotExisting<Idle>();

    Destroy(this);
  }
}

