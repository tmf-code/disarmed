using System;
using UnityEngine;

public class Handedness : MonoBehaviour
{
  public HandTypes handType = HandTypes.HandLeft;
}

public enum HandTypes
{
  HandLeft = OVRPlugin.Hand.HandLeft,
  HandRight = OVRPlugin.Hand.HandRight,
}