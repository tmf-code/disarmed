using UnityEngine;

public class SendHandToForearmCollisionToTarget : MonoBehaviour
{
  public GameObject target;

  public enum ArmSpot
  {
    Hand,
    Forearm
  }

  public ArmSpot armSpot;

  void OnTriggerEnter(Collider collider)
  {
    TryGrab(collider.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    TryGrab(collision.gameObject);
  }

  private void TryGrab(GameObject collider)
  {
    // Other should also have this component
    if (!collider.TryGetComponent<SendHandToForearmCollisionToTarget>(out var other)) return;

    // Should not have same target (on same arm)
    if (target == other.target) return;

    // Should be opposites in hand/forearm
    if (armSpot == other.armSpot) return;

    // This should be the Hand
    if (armSpot != ArmSpot.Hand) return;

    // Both targets should have idle component
    var iHaveIdle = target.TryGetComponent<Idle>(out var targetIdle);
    var otherHasIdle = other.target.TryGetComponent<Idle>(out var otherIdle);
    if (!iHaveIdle || !otherHasIdle) return;

    // Since I will perform the grab, I should be a User
    var iAmUser = target.HasComponent<PlayerArmBehaviour>();
    if (!iAmUser) return;

    targetIdle.OnGrabBegin(otherIdle);
  }

}