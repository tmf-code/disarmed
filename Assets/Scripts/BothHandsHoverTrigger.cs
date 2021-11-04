using UnityEngine;

public class BothHandsHoverTrigger : MonoBehaviour

{
  public HandHoverTrigger rightHand;
  public HandHoverTrigger leftHand;
  public bool isHoverComplete = false;

  void Update()
  {
    isHoverComplete = rightHand.isHoverComplete && leftHand.isHoverComplete;

    if (!isHoverComplete) return;

    rightHand.transform.FindRecursiveOrThrow("r_handMeshNode")
      .GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
    leftHand.transform.FindRecursiveOrThrow("l_handMeshNode")
      .GetComponent<MeshRenderer>().material
      .SetColor("_Color", Color.green);
  }
}
