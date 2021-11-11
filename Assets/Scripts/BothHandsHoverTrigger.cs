using UnityEngine;
using UnityEngine.UI;

public class BothHandsHoverTrigger : MonoBehaviour

{
  public HandHoverTrigger rightHand;
  public HandHoverTrigger leftHand;
  public Image indicator = null;
  public bool isHoverComplete = false;
  public bool isHoveringBoth = false;
  private float progression = 0f;
  public float hoverDuration = 4f;

  public AudioSource[] sounds;
  private bool hasStarted = false;
  private bool hasFinished = false;


  void Start()
  {
    sounds = GetComponents<AudioSource>();
  }

  void Update()
  {
    if (rightHand.inHoverPosition && leftHand.inHoverPosition && !isHoverComplete)
    {
      isHoveringBoth = true;
      progression += Time.deltaTime;
      rightHand.setProgression(progression / hoverDuration);
      leftHand.setProgression(progression / hoverDuration);
      indicator.fillAmount = progression / hoverDuration;

      if (!hasStarted)
      {
        sounds[0].Play();
        hasStarted = true;
      }

    }
    else if (!isHoverComplete)
    {
      isHoveringBoth = false;
      progression = 0;
      indicator.fillAmount = 0;

      hasStarted = false;
      sounds[0].Stop();

    }
    isHoverComplete = progression >= hoverDuration;

    if (!isHoverComplete) return;

    indicator.fillAmount = 0;
    rightHand.GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
    leftHand.GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);

    if (!hasFinished)
    {
      sounds[0].Stop();
      sounds[1].Play();
      hasFinished = true;
    }

  }
}
