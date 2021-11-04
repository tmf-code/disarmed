using UnityEngine;

public class HandHoverTrigger : MonoBehaviour
{
  public MeshRenderer meshRenderer;
  public Collider playerHand;
  public bool isHoverComplete = false;
  /// <summary>
  /// Seconds
  /// </summary>
  public float hoverDuration = 4F;
  private float hoverProgression = 0f;

  void Start()
  {
    meshRenderer.material.SetColor("_Color", Color.red);
  }

  private void OnTriggerStay(Collider other)
  {
    if (other != playerHand) return;
    if (isHoverComplete) return;
    if (hoverProgression >= hoverDuration)
    {
      isHoverComplete = true;
      return;
    }


    hoverProgression += Time.deltaTime;
    meshRenderer.material.SetColor("_Color", Color.Lerp(Color.red, Color.blue, hoverProgression));
  }

  private void OnTriggerExit(Collider other)
  {
    if (isHoverComplete) return;

    meshRenderer.material.SetColor("_Color", Color.red);
    hoverProgression = 0f;
  }
}
