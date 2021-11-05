using UnityEngine;

public class HandHoverTrigger : MonoBehaviour
{
  public MeshRenderer meshRenderer;
  public ChildDictionary playerHand;
  public bool isHoverComplete = false;
  /// <summary>
  /// Seconds
  /// </summary>
  public float hoverDuration = 4F;
  private float hoverProgression = 0f;
  private Collider modelCollider;

  void Start()
  {
    modelCollider = playerHand.model.GetComponentOrThrow<Collider>();
    meshRenderer.material.SetColor("_Color", Color.red);
  }

  private void OnTriggerStay(Collider other)
  {
    if (other != modelCollider) return;
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
