using System.Collections.Generic;
using UnityEngine;

public class DetectHumerusShoulderCollision : MonoBehaviour
{
  public GameObject target;
  public Dictionary<CollisionPair, RemainingCollision> startedAttachments = new Dictionary<CollisionPair, RemainingCollision>();

  void OnTriggerEnter(Collider collider)
  {
    TryStartAttachment(collider.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    TryStartAttachment(collision.gameObject);
  }

  void OnTriggerExit(Collider collider)
  {
    TryCancelAttachment(collider.gameObject);
  }

  void OnCollisionExit(Collision collision)
  {
    TryCancelAttachment(collision.gameObject);
  }

  private void TryStartAttachment(GameObject otherGameObject)
  {
    // Only able to attach arms after act 4
    var maybeTimeline = GameObject.Find("Timeline");
    if (maybeTimeline)
    {
      var act = maybeTimeline.GetComponent<Timeline>().act;
      if (act < Timeline.Acts.Four) return;
    }

    var otherIsShoulder = otherGameObject.CompareTag("LeftShoulder") || otherGameObject.CompareTag("RightShoulder");
    if (!otherIsShoulder) return;

    var thisIsHumerus = gameObject.CompareTag("Humerus");
    if (!thisIsHumerus) return;

    var handedness = target.GetComponentOrThrow<Handedness>();
    var isLeft = handedness.IsLeft();

    var shoulderAndArmAreSameSide = isLeft
      ? otherGameObject.CompareTag("LeftShoulder")
      : otherGameObject.CompareTag("RightShoulder");

    if (!shoulderAndArmAreSameSide) return;

    // Since I will perform the attachment, the arm getting ready to put on should be world behaviour
    var isAmWorld = target.HasComponent<WorldArmBehaviour>();
    if (!isAmWorld) return;

    // Player arm slot should be free
    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    if (playerArms.HasHand(handedness.handType)) return;

    // This arm should be Grabbed
    if (!target.TryGetComponent(out Grabbed grabbed)) return;

    var grabbing = grabbed.other;

    var collisionPair = new CollisionPair(target, otherGameObject);
    var remainingCollision = new RemainingCollision();

    startedAttachments.Add(collisionPair, remainingCollision);
    grabbing.OnAttachBegin(() => remainingCollision.isColliding);
  }

  private void TryCancelAttachment(GameObject otherGameObject)
  {
    var pair = new CollisionPair(target, otherGameObject);
    if (!startedAttachments.TryGetValue(pair, out var remainingCollision)) return;

    remainingCollision.isColliding = false;
    startedAttachments.Remove(pair);
  }
}
