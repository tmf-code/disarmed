using System;
using UnityEngine;

public class Handedness : MonoBehaviour
{
  public CustomHand.HandTypes handType = CustomHand.HandTypes.None;

  void Start()
  {
    if (handType == CustomHand.HandTypes.None) throw new Exception("Must set hand type");
  }
}
