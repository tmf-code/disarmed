using UnityEngine;

public class Grab : MonoBehaviour
{

  public enum GrabState
  {
    IDLE,
    GRABBING,
    GRABBED,
  }

  public GrabState state = GrabState.IDLE;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    switch (state)
    {
      case GrabState.IDLE:
        {
          transform.GetComponent<InverseKinematics>().enabled = true;
          transform.GetComponent<OVRCustomSkeleton>().enabled = true;
          break;
        }

      case GrabState.GRABBING:
        {
          transform.GetComponent<InverseKinematics>().enabled = true;
          transform.GetComponent<OVRCustomSkeleton>().enabled = false;
          break;
        }

      case GrabState.GRABBED:
        {
          transform.GetComponent<InverseKinematics>().enabled = false;
          transform.GetComponent<OVRCustomSkeleton>().enabled = false;
          break;
        }
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.transform.parent == null)
    {
      Debug.Log("No parent");
      return;
    }
    if (collider.transform.parent.gameObject == gameObject)
    {
      Debug.Log($"trigger enter with self");
      return;
    }
    Debug.Log($"trigger enter: {collider}");
  }
  void OnTriggerExit(Collider collider)
  {
    if (collider.transform.parent == null)
    {
      Debug.Log("No parent");
      return;
    }

    if (collider.transform.parent.gameObject)
    {
      Debug.Log($"trigger exit with self");
      return;
    }
    Debug.Log($"trigger exit: {collider}");
  }
}
