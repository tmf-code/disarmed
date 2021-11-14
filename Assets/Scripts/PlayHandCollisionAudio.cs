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
    var iAmUser = target.GetComponentOrThrow<ArmBehaviour>().owner == ArmBehaviour.ArmOwnerType.User;
    if (!iAmUser) return;

    var maybeAudioController = GameObject.Find("AudioController");
    if (!maybeAudioController) return;

    var audioPlayer = maybeAudioController.GetComponentOrThrow<AudioPlayer>();
    audioPlayer.PlayEffectAtPosition(AudioPlayer.SoundEffects.Collision, transform.position);
  }
}