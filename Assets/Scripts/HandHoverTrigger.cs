using UnityEngine;

public class HandHoverTrigger : MonoBehaviour
{
  public MeshRenderer meshRenderer;
  public ChildDictionary playerHand;
  public AudioSource source;
  private Collider modelCollider;
  public bool inHoverPosition = false;

  private Color active = new Color(1F, 1F, 1F, 0.3F);
  private Color inactive = new Color(1F, 1F, 1F, 0.1F);
  void Start()
  {
    modelCollider = playerHand.model.gameObject.GetComponentOrThrow<Collider>();
    meshRenderer.material.SetColor("_Color", inactive);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other != modelCollider) return;
    source.Play();

    meshRenderer.material.SetColor("_Color", active);

    transform.localScale = new Vector3(1.03F, 1.03F, 1.03F);
    Invoke(nameof(MakeOriginalSize), 0.2F);
  }



  private void OnTriggerStay(Collider other)
  {
    if (other != modelCollider) return;
    inHoverPosition = true;

  }

  private void OnTriggerExit(Collider other)
  {
    inHoverPosition = false;
    meshRenderer.material.SetColor("_Color", inactive);
  }

  private void MakeOriginalSize()
  {
    transform.localScale = new Vector3(1.0F, 1.0F, 1.0F);
  }
}
