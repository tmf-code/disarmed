using System;
using UnityEngine;

public class PinchState : MonoBehaviour
{
  [Serializable]
  public class PinchStateDisplay
  {
    public bool thumbActive = false;
    public float thumbStrength = 0;
    public bool indexActive = false;
    public float indexStrength = 0;
    public bool middleActive = false;
    public float middleStrength = 0;
    public bool ringActive = false;
    public float ringStrength = 0;
    public bool pinkyActive = false;
    public float pinkyStrength = 0;
  }

  public CustomHand hand;
  public PinchStateDisplay pinchState = new PinchStateDisplay();

  void Update()
  {
    pinchState.thumbActive = hand.GetFingerIsPinching(CustomHand.HandFinger.Thumb);
    pinchState.thumbStrength = hand.GetFingerPinchStrength(CustomHand.HandFinger.Thumb);

    pinchState.indexActive = hand.GetFingerIsPinching(CustomHand.HandFinger.Index);
    pinchState.indexStrength = hand.GetFingerPinchStrength(CustomHand.HandFinger.Index);

    pinchState.middleActive = hand.GetFingerIsPinching(CustomHand.HandFinger.Middle);
    pinchState.middleStrength = hand.GetFingerPinchStrength(CustomHand.HandFinger.Middle);

    pinchState.ringActive = hand.GetFingerIsPinching(CustomHand.HandFinger.Ring);
    pinchState.ringStrength = hand.GetFingerPinchStrength(CustomHand.HandFinger.Ring);

    pinchState.pinkyActive = hand.GetFingerIsPinching(CustomHand.HandFinger.Pinky);
    pinchState.pinkyStrength = hand.GetFingerPinchStrength(CustomHand.HandFinger.Pinky);
  }
}
