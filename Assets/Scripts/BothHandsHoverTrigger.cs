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

  void Update()
  {
    if (rightHand.inHoverPosition && leftHand.inHoverPosition && !isHoverComplete)
    {
      isHoveringBoth = true;
      progression += Time.deltaTime;
      rightHand.setProgression(progression / hoverDuration);
      leftHand.setProgression(progression / hoverDuration);
      indicator.fillAmount = progression / hoverDuration;
    }
    else if (!isHoverComplete)
    {
      isHoveringBoth = false;
      progression = 0;
      indicator.fillAmount = 0;
    }
    isHoverComplete = progression >= hoverDuration;

    if (!isHoverComplete) return;

    indicator.fillAmount = 0;
    rightHand.GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
    leftHand.GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
  }
}
