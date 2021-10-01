using UnityEngine;

public class Idle : MonoBehaviour
{
  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = true;
    transform.GetComponent<CustomSkeleton>().enabled = true;
  }
}

