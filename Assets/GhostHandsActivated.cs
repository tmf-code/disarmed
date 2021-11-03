using UnityEngine;

public class GhostHandsActivated : MonoBehaviour

{
  public HandTrigger rightHand;
  public HandTrigger leftHand;

  // Update is called once per frame
  void Update()
  {
    if (!rightHand.isOnGhostHand && !leftHand.isOnGhostHand) return;
    rightHand.transform.FindRecursiveOrThrow("r_handMeshNode")
      .GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
    leftHand.transform.FindRecursiveOrThrow("l_handMeshNode")
      .GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
  }
}
