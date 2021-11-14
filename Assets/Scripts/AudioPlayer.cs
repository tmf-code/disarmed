using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
  public AudioSource source;
  public EmitterPool emitterPool;

  public AudioClip intro;
  public AudioClip act1;
  public AudioClip act2;
  public AudioClip act3;
  public AudioClip act4;
  public AudioClip chime;

  public enum ActAudio
  {
    Intro,
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

  public AudioClip collision;

  public void PlayAct(ActAudio act)
  {
    switch (act)
    {
      case ActAudio.Intro:
        source.clip = intro;
        break;
      case ActAudio.Chime:
        source.clip = chime;
        break;
      case ActAudio.Act1:
        source.clip = act1;
        break;
      case ActAudio.Act2:
        source.clip = act2;
        break;
      case ActAudio.Act3:
        source.clip = act3;
        break;
      case ActAudio.Act4:
        source.clip = act4;
        break;
    }
    source.Play();
  }

  public void PlayEffectAtPosition(SoundEffects effect, Vector3 position)
  {
    var clip = effect switch
    {
      SoundEffects.Collision => collision,
      _ => throw new System.NotImplementedException(),
    };

    emitterPool.TryPlayAtPosition(clip, position);
  }
}
