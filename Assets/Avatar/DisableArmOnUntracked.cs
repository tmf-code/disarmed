using UnityEngine;

public class DisableArmOnUntracked : MonoBehaviour
{
  private CustomHand hand;
  private CustomSkeleton skeleton;

  void Start()
  {
    hand = gameObject.GetComponent<CustomHand>();
    skeleton = gameObject.GetComponent<CustomSkeleton>();
  }


  void Update()
  {
    if (hand.isTracked)
    {
      gameObject.GetComponent<InverseKinematics>().enabled = true;
      gameObject.GetComponent<HandCollider>().enabled = true;
      gameObject.GetComponent<ForearmCollider>().enabled = true;
      gameObject.GetComponent<HumerusCollider>().enabled = true;
      gameObject.GetComponent<GestureState>().enabled = true;
      gameObject.GetComponent<PinchState>().enabled = true;

      skeleton.updateRootPose = true;
      skeleton.updateRootScale = true;

      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(true);
      }
    }
    else
    {
      gameObject.GetComponent<InverseKinematics>().enabled = false;
      gameObject.GetComponent<HandCollider>().enabled = false;
      gameObject.GetComponent<ForearmCollider>().enabled = false;
      gameObject.GetComponent<HumerusCollider>().enabled = false;
      gameObject.GetComponent<GestureState>().enabled = false;
      gameObject.GetComponent<PinchState>().enabled = false;

      skeleton.updateRootPose = false;
      skeleton.updateRootScale = false;

      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(false);
      }
    }
  }
}
