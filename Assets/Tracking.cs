using UnityEngine;

public class Tracking : MonoBehaviour
{
  public GameObject left;
  public GameObject right;

  public bool swap;
  public void SetSwapped(bool isSwapped)
  {
    left.GetComponentOrThrow<HandTracking>().isSwapped = isSwapped;
    right.GetComponentOrThrow<HandTracking>().isSwapped = isSwapped;
  }

  public GameObject GetSide(Handedness hand)
  {
    if (hand.IsLeft()) return left;
    return right;
  }

  void FixedUpdate()
  {

    SetSwapped(swap);

  }

}
