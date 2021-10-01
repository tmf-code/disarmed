using System;
using UnityEngine;

public class Handedness : MonoBehaviour
{
  public OVRHand.Hand handType = OVRHand.Hand.None;

  void Start()
  {
    if (handType == OVRHand.Hand.None) throw new Exception("Must set hand type");
  }
}
