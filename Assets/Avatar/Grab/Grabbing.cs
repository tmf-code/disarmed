using UnityEngine;

public class Grabbing : MonoBehaviour
{
  public GameObject grabbing;

  void Start()
  {
    transform.GetComponent<InverseKinematics>().enabled = true;
    var skeleton = transform.GetComponent<CustomSkeleton>();
    skeleton.enabled = true;
    skeleton.updateBones = false;
  }
}

