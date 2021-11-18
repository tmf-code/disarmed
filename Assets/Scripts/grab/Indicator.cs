using System;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
  public Transform target;
  public new Transform camera;

  public AudioSource source;
  public SuccessSound success = SuccessSound.GrabArm;
  public AudioClip hum;
  public AudioClip grabArm;
  public AudioClip dropArm;
  public AudioClip putOnArm;

  public enum SuccessSound
  {
    Hum,
    GrabArm,
    DropArm,
    PutOnArm,
  }

  public bool isComplete = false;

  public float lerpSpeed = 0.02f;
  public Func<float> progression = () => 0F;
  public Image image;

  void Start()
  {

    var position = target.position;
    camera = Camera.main.transform;
    var rotation = Quaternion.Euler(camera.rotation.x, camera.rotation.y, 0);
    transform.SetPositionAndRotation(position, rotation);

    source.clip = hum;
    source.volume = 0;
    source.FadeIn(1F);
    source.Play();
  }

  public void FadeOutAudio()
  {
    source.FadeOut(0.1F);
  }

  void Update()
  {
    var position = Vector3.Lerp(transform.position, target.position, lerpSpeed);
    var quaternion = Quaternion.Slerp(transform.rotation, camera.rotation, lerpSpeed);
    var eulers = quaternion.eulerAngles;
    var clippedRotation = Quaternion.Euler(eulers.x, eulers.y, 0);
    transform.SetPositionAndRotation(position, clippedRotation);

    if (isComplete)
    {
      Destroy(gameObject, source.clip.length + 0.2F);
      return;
    }

    var progress = Mathf.Clamp(progression(), 0, 1);

    if (progress >= 1F)
    {
      source.Stop();
      source.clip = GetSuccessSound();
      source.Play();
      isComplete = true;
    }

    image.fillAmount = progress;
  }

  private AudioClip GetSuccessSound() => success switch
  {
    SuccessSound.Hum => hum,
    SuccessSound.GrabArm => grabArm,
    SuccessSound.DropArm => dropArm,
    SuccessSound.PutOnArm => putOnArm,
    _ => throw new NotImplementedException(),
  };
}
