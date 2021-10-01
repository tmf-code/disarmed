using UnityEngine;

public class Idle : MonoBehaviour
{
  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;
  void Start()
  {
    creationTime = Time.time;
    transform.GetComponent<InverseKinematics>().enabled = true;
    transform.GetComponent<CustomSkeleton>().enabled = true;
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
}

