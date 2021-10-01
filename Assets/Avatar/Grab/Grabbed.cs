using UnityEngine;

public class Grabbed : MonoBehaviour
{
  public GameObject grabbedBy;

  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = false;
    transform.GetComponent<CustomSkeleton>().enabled = false;
  }

  void Update()
  {
    transform.SetPositionAndRotation(grabbedBy.transform.position, grabbedBy.transform.rotation);
  }
}

