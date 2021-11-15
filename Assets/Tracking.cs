using UnityEngine;

public class Tracking : MonoBehaviour
{
  public GameObject left;
  public GameObject right;

#if UNITY_EDITOR
  [Button(nameof(Swap))]
  public bool buttonField;

  [Button(nameof(UnSwap))]
  public bool buttonField2;

  public void Swap()
  {
    SetSwapped(true);
  }

  public void UnSwap()
  {
    SetSwapped(false);
  }
#endif

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
}
