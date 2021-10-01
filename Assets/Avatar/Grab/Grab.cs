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

  }

  void SetState(GrabState state, SetStateData data)
  {
    if (enabled == false) return;
    switch (state)
    {
      case GrabState.IDLE:
        {
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();
          gameObject.AddIfNotExisting<Idle>();
          break;
        }

      case GrabState.GRABBING:
        {
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Idle>();
          var grabbing = gameObject.AddIfNotExisting<Grabbing>();
          grabbing.grabbing = data.grabbing;
          break;
        }

      case GrabState.GRABBED:
        {
          gameObject.RemoveComponent<Grabbing>();
          gameObject.RemoveComponent<Idle>();
          var grabbed = gameObject.AddIfNotExisting<Grabbed>();
          grabbed.grabbedBy = data.grabbedBy;
          break;
        }
    }

    this.state = state;
  }

  public void OnTriggersEnter(TwoPartyCollider colliders)
  {
    if (colliders.other.transform.parent == null)
    {
      // Debug.Log("No parent");
      return;
    }
    if (colliders.other.transform.parent.gameObject == gameObject)
    {
      // Debug.Log($"trigger enter with self");
      return;
    }

    var source = colliders.source;
    var other = colliders.other;

    switch (state)
    {
      case GrabState.IDLE:
        if (source.CompareTag("Hand") && other.CompareTag("Forearm"))
          SetState(GrabState.GRABBING, new SetStateData(other.gameObject, null));
        if (source.CompareTag("Forearm") && other.CompareTag("Hand"))
          SetState(GrabState.GRABBED, new SetStateData(null, other.gameObject));
        break;

      case GrabState.GRABBING:
        break;

      case GrabState.GRABBED:
        break;
    }


    // Debug.Log($"trigger enter: {colliders.ToString()}");
  }

}

public struct SetStateData
{
#nullable enable
  public GameObject? grabbing;
  public GameObject? grabbedBy;

  public SetStateData(GameObject? grabbing, GameObject? grabbedBy)
  {
    this.grabbing = grabbing;
    this.grabbedBy = grabbedBy;
  }
#nullable disable
}

