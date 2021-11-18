using System.Collections.Generic;
using UnityEngine;

public class SendHandToForearmCollisionToTarget : MonoBehaviour
{
  public GameObject target;
  public Dictionary<CollisionPair, RemainingCollision> startedGrabs = new Dictionary<CollisionPair, RemainingCollision>();

  public enum ArmSpot
  {
    Hand,
    Forearm
  }

  public ArmSpot armSpot;

  void OnTriggerEnter(Collider collider)
  {
    TryStartGrab(collider.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    TryStartGrab(collision.gameObject);
  }

  void OnTriggerExit(Collider collider)
  {
    TryCancelGrab(collider.gameObject);
  }

  void OnCollisionExit(Collision collision)
  {
    TryCancelGrab(collision.gameObject);
  }

  private void TryStartGrab(GameObject otherGameObject)
  {
    // Since I will perform the grab, I should be a User
    var iAmUser = target.HasComponent<PlayerArmBehaviour>();
    if (!iAmUser) return;

    var isInteractive = target.HasComponent<PlayerInteracts>();
    if (!isInteractive) return;

    // Other should also have this component
    if (!otherGameObject.TryGetComponent<SendHandToForearmCollisionToTarget>(out var other)) return;

    // Should not have same target (on same arm)
    if (target == other.target) return;

    // Should be opposites in hand/forearm
    if (armSpot == other.armSpot) return;

    // This should be the Hand
    if (armSpot != ArmSpot.Hand) return;


    // I should be awaiting grabbing, and the other grabbed
    if (!GrabOperations.TryGetGrabbingPair(target, other.target, out var awaitingGrabbing, out var awaitingGrabbed)) return;

    var collisionPair = new CollisionPair(target, other.target);
    var remainingCollision = new RemainingCollision();
    startedGrabs.Add(collisionPair, remainingCollision);
    awaitingGrabbing.OnGrabBegin(awaitingGrabbed, () => remainingCollision.isColliding);
  }

  private void TryCancelGrab(GameObject otherGameObject)
  {
    // Other should also have this component
    if (!otherGameObject.TryGetComponent<SendHandToForearmCollisionToTarget>(out var other)) return;
    var pair = new CollisionPair(target, other.target);
    if (!startedGrabs.TryGetValue(pair, out var remainingCollision)) return;

    remainingCollision.isColliding = false;
    startedGrabs.Remove(pair);
  }
}
