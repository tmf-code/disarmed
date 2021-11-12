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
      gameObject.GetOptionComponent<ApplyVRTrackingDataToModelRagdoll>().Map(c => c.enabled = true);
      gameObject.GetOptionComponent<ApplyRootTracking>().Map(c => c.enabled = true);
      gameObject.GetOptionComponent<ApplyHandTracking>().Map(c => c.enabled = true);
    }
    else
    {
      gameObject.GetOptionComponent<ApplyVRTrackingDataToModelRagdoll>().Map(c => c.enabled = false);
      gameObject.GetOptionComponent<ApplyRootTracking>().Map(c => c.enabled = false);
      gameObject.GetOptionComponent<ApplyHandTracking>().Map(c => c.enabled = false);
    }
  }
}
