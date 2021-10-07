using UnityEngine;

public class Idle : MonoBehaviour
{
  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  void Start()
  {
    creationTime = Time.time;
    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;

    gameObject.RemoveComponent<Grabbed>();
    gameObject.RemoveComponent<Grabbing>();
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

  public void OnTriggersEnter(TwoPartyCollider colliders)
  {
    if (!canTransition) return;

    if (colliders.other.transform.parent == null)
    {
      // Debug.Log("No parent");
      return;
    }

    var otherParent = colliders.other.transform.parent.gameObject;
    if (otherParent == gameObject)
    {
      // Debug.Log($"trigger enter with self");
      return;
    }

    var source = colliders.source;
    var other = colliders.other;


    if (source.CompareTag("Hand") && other.CompareTag("Forearm"))
    {
      var grabbing = gameObject.AddIfNotExisting<Grabbing>();
      var grabbed = otherParent.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }

    if (source.CompareTag("Forearm") && other.CompareTag("Hand"))
    {
      var grabbing = otherParent.AddIfNotExisting<Grabbing>();
      var grabbed = gameObject.AddIfNotExisting<Grabbed>();
      grabbing.grabbed = grabbed;
      grabbed.grabbing = grabbing;
      Destroy(this);
      return;
    }
  }
}