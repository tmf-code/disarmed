using UnityEngine;

public class HandHoverTrigger : MonoBehaviour
{
  public MeshRenderer meshRenderer;
  public ChildDictionary playerHand;
  private Collider modelCollider;
  public bool inHoverPosition = false;

  void Start()
  {
    modelCollider = playerHand.model.gameObject.GetComponentOrThrow<Collider>();
    meshRenderer.material.SetColor("_Color", Color.red);
  }

  private void OnTriggerEnter(Collider other)
  {
    meshRenderer.material.SetColor("_Color", Color.cyan);
  }

  private void OnTriggerStay(Collider other)
  {
    if (other != modelCollider) return;
    inHoverPosition = true;
  }

  private void OnTriggerExit(Collider other)
  {
    inHoverPosition = false;
    meshRenderer.material.SetColor("_Color", Color.red);
  }

  public void setProgression(float progression)
  {
    meshRenderer.material.SetColor("_Color", Color.Lerp(Color.cyan, Color.blue, progression));
  }
}
