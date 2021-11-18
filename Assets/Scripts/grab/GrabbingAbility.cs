using UnityEngine;

public class GrabbingAbility : MonoBehaviour
{
#if UNITY_EDITOR
  [Button(nameof(OpenHand))]
  public bool buttonField;
  public void OpenHand()
  {
    var grabbing = gameObject.GetComponent<Grabbing>();
    var grabbed = grabbing.other;
    GrabOperations.GrabToReleaseIntent(grabbing, grabbed);
  }

#endif
}


