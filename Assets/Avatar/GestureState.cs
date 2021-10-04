using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GestureState : MonoBehaviour
{
  [Serializable]
  public class GestureStateDisplay
  {
    public bool thumbOpen = false;
    public bool indexOpen = false;
    public bool middleOpen = false;
    public bool ringOpen = false;
    public bool pinkyOpen = false;

    public bool thumbClosed = false;
    public bool indexClosed = false;
    public bool middleClosed = false;
    public bool ringClosed = false;
    public bool pinkyClosed = false;

    public float thumbStraightness = 0;
    public float indexStraightness = 0;
    public float middleStraightness = 0;
    public float ringStraightness = 0;
    public float pinkyStraightness = 0;

  }

  public enum FingerNames
  {
    thumb,
    index,
    middle,
    ring,
    pinky
  }

  [Serializable]
  public class Fingers
  {
    public Finger thumb;
    public Finger index;
    public Finger middle;
    public Finger ring;
    public Finger pinky;

    public Fingers(Finger thumb, Finger index, Finger middle, Finger ring, Finger pinky)
    {
      this.thumb = thumb;
      this.index = index;
      this.middle = middle;
      this.ring = ring;
      this.pinky = pinky;
    }

    public void SetFinger(FingerNames name, Finger value)
    {
      switch (name)
      {
        case FingerNames.thumb:
          thumb = value;
          break;
        case FingerNames.index:
          index = value;
          break;
        case FingerNames.middle:
          middle = value;
          break;
        case FingerNames.ring:
          ring = value;
          break;
        case FingerNames.pinky:
          pinky = value;
          break;
      }
    }
  }

  [Serializable]
  public class Finger
  {
    public Transform[] bones;
    private readonly float minStraightness;
    private readonly float maxStraightness;

    public Finger(Transform[] bones, float minStraightness, float maxStraightness)
    {
      this.bones = bones;
      this.minStraightness = minStraightness;
      this.maxStraightness = maxStraightness;
    }

    public float Length()
    {
      return bones.Select(transform => transform.localPosition.magnitude).Sum();
    }

    public float Straightness()
    {
      var length = Length();
      var distance = Vector3.Distance(bones.First().position, bones.Last().position);

      var absoluteStraightness = distance / length;
      var scaledStraightness = absoluteStraightness.Rescale(this.minStraightness, this.maxStraightness, 0, 1);
      return scaledStraightness;
    }
  }

  public Fingers fingers;
  public Handedness handedness;
  public GestureStateDisplay gestureState = new GestureStateDisplay();

  void Start()
  {

    handedness = gameObject.GetComponentIfNull(handedness);
    var handSide = handedness.handType == CustomHand.HandTypes.HandLeft ? "l" : "r";
    var fingerNames = Enum.GetNames(typeof(FingerNames));

    // Empirically derived
    var fingerDimensions = new Dictionary<FingerNames, Range>(5)
    {
      { FingerNames.thumb, new Range(0.43F, 0.79F) },
      { FingerNames.index, new Range(0.12F, 0.46F) },
      { FingerNames.middle, new Range(0.15F, 0.50F) },
      { FingerNames.ring, new Range(0.13F, 0.49F) },
      { FingerNames.pinky, new Range(0.13F, 0.57F) },
    };

    fingerNames.ToList().ForEach(name =>
    {
      var bone1 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}1");
      var bone2 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}2");
      var bone3 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}3");
      var boneNull = transform.FindRecursiveOrThrow($"b_{handSide}_{name}_null");
      var boneEnd = transform.FindRecursiveOrThrow($"b_{handSide}_{name}_null_end");

      Enum.TryParse<FingerNames>(name, false, out var fingerName);
      fingerDimensions.TryGetValue(fingerName, out var range);
      fingers.SetFinger(fingerName,
        new Finger(new Transform[]{
          bone1,
          bone2,
          bone3,
          boneNull,
          boneEnd,
        },
        range.min,
        range.max
      ));
    });
  }

  // Update is called once per frame
  void Update()
  {
    gestureState.thumbStraightness = fingers.thumb.Straightness();
    gestureState.indexStraightness = fingers.index.Straightness();
    gestureState.middleStraightness = fingers.middle.Straightness();
    gestureState.ringStraightness = fingers.ring.Straightness();
    gestureState.pinkyStraightness = fingers.pinky.Straightness();

    gestureState.thumbOpen = gestureState.thumbStraightness > 0.8;
    gestureState.indexOpen = gestureState.indexStraightness > 0.8;
    gestureState.middleOpen = gestureState.middleStraightness > 0.8;
    gestureState.ringOpen = gestureState.ringStraightness > 0.8;
    gestureState.pinkyOpen = gestureState.pinkyStraightness > 0.8;

    gestureState.thumbClosed = gestureState.thumbStraightness < 0.3;
    gestureState.indexClosed = gestureState.indexStraightness < 0.3;
    gestureState.middleClosed = gestureState.middleStraightness < 0.3;
    gestureState.ringClosed = gestureState.ringStraightness < 0.3;
    gestureState.pinkyClosed = gestureState.pinkyStraightness < 0.3;
  }
}
