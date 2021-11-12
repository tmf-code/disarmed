using UnityEngine;

public class DetectHumerusShoulderCollision : MonoBehaviour
{
  public GameObject sendCollisionTo;

  void OnTriggerEnter(Collider collider)
  {
    // Only able to attach arms after act 4
    var maybeTimeline = GameObject.Find("Timeline");
    if (maybeTimeline)
    {
      var act = maybeTimeline.GetComponent<Timeline>().act;
      if (act < Timeline.Acts.Four) return;
    }

    var otherIsShoulder = collider.CompareTag("LeftShoulder") || collider.CompareTag("RightShoulder");
    var thisIsHumerus = gameObject.CompareTag("Humerus");
    var targetIsGrabbed = sendCollisionTo.HasComponent<Grabbed>();

    if (otherIsShoulder && thisIsHumerus && targetIsGrabbed)
    {
      sendCollisionTo.SendMessage(nameof(Grabbed.CollideWithShoulder), collider, SendMessageOptions.DontRequireReceiver);
    }
  }
}
