using UnityEngine;

public class Grabbed : MonoBehaviour
{
  public GameObject grabbedBy;

  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = false;
    transform.GetComponent<OVRCustomSkeleton>().enabled = false;
  }
}

