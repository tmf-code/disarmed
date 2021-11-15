using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
  public AudioSource source;
  public EmitterPool emitterPool;

  public AudioClip tuning;
  public AudioClip chime;
  public AudioClip act1;
  public AudioClip act2;
  public AudioClip act3;
  public AudioClip act4;

  public enum ActAudio
  {
    Tuning,
    Chime,
    Act1,
    Act2,
    Act3,
    Act4,
  }

  public enum SoundEffects
  {
    Collision,
  }

  public AudioClip collision1;
  public AudioClip collision2;
  public AudioClip collision3;
  public AudioClip collision4;
  public AudioClip collision5;
  public AudioClip collision6;
  public AudioClip collision7;
  public AudioClip collision8;
  public AudioClip collision9;

  public void PlayAct(ActAudio act)
  {
    switch (act)
    {
      case ActAudio.Tuning:
        source.loop = true;
        source.clip = tuning;
        break;
      case ActAudio.Chime:
        source.loop = false;
        source.clip = chime;
        break;
      case ActAudio.Act1:
        source.loop = false;
        source.clip = act1;
        break;
      case ActAudio.Act2:
        source.loop = false;
        source.clip = act2;
        break;
      case ActAudio.Act3:
        source.loop = false;
        source.clip = act3;
        break;
      case ActAudio.Act4:
        source.loop = false;
        source.clip = act4;
        break;
    }

    source.Play();
  }

  public void PlayEffectAtPosition(SoundEffects effect, Vector3 position)
  {
    var collisions = new AudioClip[] { collision1, collision2, collision3, collision4, collision5, collision6, collision7, collision8, collision9, };
    var clip = effect switch
    {
      SoundEffects.Collision => collisions[Random.Range(0, collisions.Length)],
      _ => throw new System.NotImplementedException(),
    };

    emitterPool.TryPlayAtPosition(clip, position);
  }
}
