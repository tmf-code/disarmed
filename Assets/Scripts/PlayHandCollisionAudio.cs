using UnityEngine;

public class PlayHandCollisionAudio : MonoBehaviour
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
    TryPlayCollisionAudio(collider.gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    TryPlayCollisionAudio(collision.gameObject);
    TryPlayGroundCollisionAudio(collision, collision.gameObject);
  }

  private void TryPlayCollisionAudio(GameObject collider)
  {
    // Other should also have this component
    if (!collider.TryGetComponent<PlayHandCollisionAudio>(out var other)) return;

    // Should not have same target (on same arm)
    if (target == other.target) return;

    // This should be the Hand
    if (armSpot != ArmSpot.Hand) return;

    // Since I will perform the grab, I should be a User
    var iAmUser = target.HasComponent<PlayerArmBehaviour>();
    if (!iAmUser) return;

    var maybeAudioController = GameObject.Find("AudioController");
    if (!maybeAudioController) return;

    var audioPlayer = maybeAudioController.GetComponentOrThrow<AudioPlayer>();
    audioPlayer.PlayEffectAtPosition(AudioPlayer.SoundEffects.Collision, transform.position);
  }

  private void TryPlayGroundCollisionAudio(Collision collision, GameObject collider)
  {
    // Should be floor
    if (!collider.CompareTag("Floor")) return;

    // Only play thuds on act four
    var maybeTimeline = GameObject.Find("Timeline");
    if (maybeTimeline)
    {
      var act = maybeTimeline.GetComponent<Timeline>().act;
      if (act < Timeline.Acts.Four) return;
    }

    var maybeAudioController = GameObject.Find("AudioController");
    if (!maybeAudioController) return;

    var audioPlayer = maybeAudioController.GetComponentOrThrow<AudioPlayer>();

    var vel = collision.relativeVelocity.magnitude;

    if (vel < 0.1F) return;

    audioPlayer.PlayEffectAtPosition(AudioPlayer.SoundEffects.Collision, transform.position, Mathf.Clamp(vel, 0, 1));
  }
}