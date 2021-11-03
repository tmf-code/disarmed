using UnityEngine;

public class HandTrigger : MonoBehaviour
{

  public MeshRenderer meshRenderer;
  public Collider playerHand;
  public bool isOnGhostHand = false;

  private float colorCounter = 0f;

  // Start is called before the first frame update
  void Start()
  {
    meshRenderer.material.SetColor("_Color", Color.red);
  }

  private void OnTriggerStay(Collider other)
  {
    if (other != playerHand) return;
    if (isOnGhostHand) return;
    if (colorCounter < 1f)
    {
      colorCounter += Time.deltaTime * 0.25f;
      meshRenderer.material.SetColor("_Color", Color.Lerp(Color.red, Color.blue, colorCounter));
    }
    else
    {
      isOnGhostHand = true;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (isOnGhostHand) return;
    meshRenderer.material.SetColor("_Color", Color.red);
    colorCounter = 0f;
  }
}
