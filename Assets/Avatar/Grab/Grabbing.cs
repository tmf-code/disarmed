using UnityEngine;

public class Grabbing : MonoBehaviour
{
  public GameObject grabbing;

  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = true;
    transform.GetComponent<OVRCustomSkeleton>().enabled = false;
  }
}

