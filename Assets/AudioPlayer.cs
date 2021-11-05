using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
  private AudioSource source;

  public AudioClip intro;
  public AudioClip act1;
  public AudioClip act2;
  public AudioClip act3;
  public AudioClip act4;

  public enum ClipType
  {
    Intro,
    Act1,
    Act2,
    Act3,
    Act4,
  }

  void Start()
  {
    source = gameObject.GetComponentOrThrow<AudioSource>();
  }

  public void PlayAct(ClipType act)
  {
    switch (act)
    {
      case ClipType.Intro:
        source.clip = intro;
        break;
      case ClipType.Act1:
        source.clip = act1;
        break;
      case ClipType.Act2:
        source.clip = act2;
        break;
      case ClipType.Act3:
        source.clip = act3;
        break;
      case ClipType.Act4:
        source.clip = act4;
        break;
    }
    source.Play();
  }
}
