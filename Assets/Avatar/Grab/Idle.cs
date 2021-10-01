using UnityEngine;

public class Idle : MonoBehaviour
{
  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = true;
    transform.GetComponent<OVRCustomSkeleton>().enabled = true;
  }
}

