using UnityEngine;

public class DisableArmOnUntracked : MonoBehaviour
{
  private CustomHand hand;

  void Start()
  {
    hand = gameObject.GetComponent<CustomHand>();
  }


  void Update()
  {
    if (hand.isDataHighConfidence)
    {
      gameObject.GetComponent<InverseKinematics>().enabled = true;
      gameObject.GetComponent<GestureState>().enabled = true;
      gameObject.GetComponent<PinchState>().enabled = true;
      gameObject.GetComponent<ApplyRootTracking>().enabled = true;
      gameObject.GetComponent<ApplyHandTracking>().enabled = true;
    }
    else
    {
      gameObject.GetComponent<InverseKinematics>().enabled = false;
      gameObject.GetComponent<GestureState>().enabled = false;
      gameObject.GetComponent<PinchState>().enabled = false;
      gameObject.GetComponent<ApplyRootTracking>().enabled = false;
      gameObject.GetComponent<ApplyHandTracking>().enabled = false;
    }
  }
}
