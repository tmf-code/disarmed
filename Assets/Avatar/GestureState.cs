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
    public bool handOpen = false;

    public bool thumbClosed = false;
    public bool indexClosed = false;
    public bool middleClosed = false;
    public bool ringClosed = false;
    public bool pinkyClosed = false;
    public bool handClosed = false;

    public float thumbStraightness = 0;
    public float indexStraightness = 0;
    public float middleStraightness = 0;
    public float ringStraightness = 0;
    public float pinkyStraightness = 0;
    public float handStraightness = 0;
    //   public float thumbMax = float.MinValue;
    //   public float indexMax = float.MinValue;
    //   public float middleMax = float.MinValue;
    //   public float ringMax = float.MinValue;
    //   public float pinkyMax = float.MinValue;
    //   public float thumbMin = float.MaxValue;
    //   public float indexMin = float.MaxValue;
    //   public float middleMin = float.MaxValue;
    //   public float ringMin = float.MaxValue;
    //   public float pinkyMin = float.MaxValue;
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
  public GestureStateDisplay gestureState;
  public CustomSkeleton customSkeleton;

  void Start()
  {
    gestureState = new GestureStateDisplay();
    customSkeleton = gameObject.GetComponentIfNull(customSkeleton);
    handedness = gameObject.GetComponentIfNull(handedness);
    var handSide = handedness.handType == CustomHand.HandTypes.HandLeft ? "l" : "r";
    var fingerNames = Enum.GetNames(typeof(FingerNames));

    // Empirically derived
    var fingerDimensions = new Dictionary<FingerNames, Range>(5)
    {
      { FingerNames.thumb, new Range(0.64F, 0.76F) },
      { FingerNames.index, new Range(0.24F, 0.40F) },
      { FingerNames.middle, new Range(0.27F, 0.44F) },
      { FingerNames.ring, new Range(0.26F, 0.44F) },
      { FingerNames.pinky, new Range(0.34F, 0.55F) },
    };

    fingerNames.ToList().ForEach(name =>
    {
      var bone1 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}1");
      var bone2 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}2");
      var bone3 = transform.FindRecursiveOrThrow($"b_{handSide}_{name}3");

      Enum.TryParse<FingerNames>(name, false, out var fingerName);
      fingerDimensions.TryGetValue(fingerName, out var range);
      fingers.SetFinger(fingerName,
        new Finger(new Transform[]{
          bone1,
          bone2,
          bone3,
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

    // Used with absoluteStraightness to emperically determine the min and max straightnesses

    // gestureState.thumbMax = Mathf.Max(gestureState.thumbStraightness, gestureState.thumbMax);
    // gestureState.indexMax = Mathf.Max(gestureState.indexStraightness, gestureState.indexMax);
    // gestureState.middleMax = Mathf.Max(gestureState.middleStraightness, gestureState.middleMax);
    // gestureState.ringMax = Mathf.Max(gestureState.ringStraightness, gestureState.ringMax);
    // gestureState.pinkyMax = Mathf.Max(gestureState.pinkyStraightness, gestureState.pinkyMax);

    // gestureState.thumbMin = Mathf.Min(gestureState.thumbStraightness, gestureState.thumbMin);
    // gestureState.indexMin = Mathf.Min(gestureState.indexStraightness, gestureState.indexMin);
    // gestureState.middleMin = Mathf.Min(gestureState.middleStraightness, gestureState.middleMin);
    // gestureState.ringMin = Mathf.Min(gestureState.ringStraightness, gestureState.ringMin);
    // gestureState.pinkyMin = Mathf.Min(gestureState.pinkyStraightness, gestureState.pinkyMin);

    gestureState.handStraightness = (
      gestureState.thumbStraightness +
      gestureState.indexStraightness +
      gestureState.middleStraightness +
      gestureState.ringStraightness +
      gestureState.pinkyStraightness
      ) / 5;

    gestureState.thumbOpen = gestureState.thumbStraightness > 0.8;
    gestureState.indexOpen = gestureState.indexStraightness > 0.8;
    gestureState.middleOpen = gestureState.middleStraightness > 0.8;
    gestureState.ringOpen = gestureState.ringStraightness > 0.8;
    gestureState.pinkyOpen = gestureState.pinkyStraightness > 0.8;
    var handOpen = gestureState.handStraightness > 0.8;
    if (handOpen != gestureState.handOpen)
    {
      gameObject.SendMessage("OnHandOpen", SendMessageOptions.DontRequireReceiver);
    }
    gestureState.handOpen = handOpen;

    gestureState.thumbClosed = gestureState.thumbStraightness < 0.3;
    gestureState.indexClosed = gestureState.indexStraightness < 0.3;
    gestureState.middleClosed = gestureState.middleStraightness < 0.3;
    gestureState.ringClosed = gestureState.ringStraightness < 0.3;
    gestureState.pinkyClosed = gestureState.pinkyStraightness < 0.3;
    gestureState.handClosed = gestureState.handStraightness < 0.3;
    var handClosed = gestureState.handStraightness > 0.8;
    if (handClosed != gestureState.handClosed)
    {
      gameObject.SendMessage("OnHandClosed", SendMessageOptions.DontRequireReceiver);
    }
  }
}
