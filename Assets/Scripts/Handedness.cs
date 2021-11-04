using UnityEngine;

public class Handedness : MonoBehaviour
{
  public HandTypes handType = HandTypes.HandLeft;

  public bool IsLeft() => handType == HandTypes.HandLeft;
  public bool IsRight() => handType == HandTypes.HandRight;
  public string HandPrefix() => handType == HandTypes.HandLeft ? "l" : "r";

  public enum HandTypes
  {
    HandLeft = OVRPlugin.Hand.HandLeft,
    HandRight = OVRPlugin.Hand.HandRight,
  }
}

