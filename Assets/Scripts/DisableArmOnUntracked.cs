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
      gameObject.GetOptionComponent<InverseKinematics>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<GestureState>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<PinchState>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<ApplyInverseKinematics>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<ApplyVRTrackingDataToModelRagdoll>().Map(component => component.enabled = true);
      gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.enabled = true);
    }
    else
    {
      gameObject.GetOptionComponent<InverseKinematics>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<GestureState>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<PinchState>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<ApplyInverseKinematics>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<ApplyVRTrackingDataToModelRagdoll>().Map(component => component.enabled = false);
      gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.enabled = false);
    }
  }
}
