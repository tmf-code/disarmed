using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GestureState : MonoBehaviour
{
  private Fingers fingers = new Fingers();
  private Handedness handedness;
  private CustomSkeleton customSkeleton;

  public SendMessageDisplay sendMessageDisplay = new SendMessageDisplay();

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

  void Start()
  {
    customSkeleton = gameObject.GetComponentIfNull(customSkeleton);
    handedness = gameObject.GetComponentIfNull(handedness);
    var handSide = handedness.handType == CustomHand.HandTypes.HandLeft ? "l" : "r";
    var fingerNames = Enum.GetNames(typeof(FingerNames));
    fingers = new Fingers();
    sendMessageDisplay = new SendMessageDisplay();

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
      var bone1 = $"b_{handSide}_{name}1";
      var bone2 = $"b_{handSide}_{name}2";
      var bone3 = $"b_{handSide}_{name}3";

      Enum.TryParse<FingerNames>(name, false, out var fingerName);
      fingerDimensions.TryGetValue(fingerName, out var range);
      fingers.SetFinger(fingerName,
        new Finger(customSkeleton, new string[]{
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
    thumbStraightness = fingers.thumb.Straightness();
    indexStraightness = fingers.index.Straightness();
    middleStraightness = fingers.middle.Straightness();
    ringStraightness = fingers.ring.Straightness();
    pinkyStraightness = fingers.pinky.Straightness();

    handStraightness = (
      thumbStraightness +
      indexStraightness +
      middleStraightness +
      ringStraightness +
      pinkyStraightness
      ) / 5;

    thumbOpen = thumbStraightness > 0.8;
    indexOpen = indexStraightness > 0.8;
    middleOpen = middleStraightness > 0.8;
    ringOpen = ringStraightness > 0.8;
    pinkyOpen = pinkyStraightness > 0.8;
    var handOpen = handStraightness > 0.8;
    if (handOpen != this.handOpen)
    {
      gameObject.SendMessage("OnHandOpen", SendMessageOptions.DontRequireReceiver);
      var gesturePosition = customSkeleton.GetBoneFromBoneName(fingers.index.boneNames[2])?.alwaysUpdatesTransform;
      if (gesturePosition != null) sendMessageDisplay.AddMessage("OnHandOpen", gesturePosition);
    }
    this.handOpen = handOpen;

    thumbClosed = thumbStraightness < 0.3;
    indexClosed = indexStraightness < 0.3;
    middleClosed = middleStraightness < 0.3;
    ringClosed = ringStraightness < 0.3;
    pinkyClosed = pinkyStraightness < 0.3;
    var handClosed = handStraightness < 0.3;
    if (handClosed != this.handClosed)
    {
      gameObject.SendMessage("OnHandClosed", SendMessageOptions.DontRequireReceiver);
      var gesturePosition = customSkeleton.GetBoneFromBoneName(fingers.index.boneNames[2])?.alwaysUpdatesTransform;
      if (gesturePosition != null) sendMessageDisplay.AddMessage("OnHandClosed", gesturePosition);
    }
    this.handClosed = handClosed;
  }

  [Serializable]
  public class Finger
  {
    public string[] boneNames;
    public CustomSkeleton customSkeleton;
    private readonly float minStraightness;
    private readonly float maxStraightness;

    public Finger(CustomSkeleton customSkeleton, string[] boneNames, float minStraightness, float maxStraightness)
    {
      this.customSkeleton = customSkeleton;
      this.boneNames = boneNames;
      this.minStraightness = minStraightness;
      this.maxStraightness = maxStraightness;
    }

    public float Length(Transform[] bones) => bones.Select(bone => bone.localPosition.magnitude).Sum();

    public float Straightness()
    {
      var boneTransforms = BoneTransforms();
      if (boneTransforms.Length == 0) return 0;

      var absoluteStraightness = AbsoluteStraightness(boneTransforms);
      var scaledStraightness = absoluteStraightness.Rescale(minStraightness, maxStraightness, 0, 1);

      return scaledStraightness;
    }

    private Transform[] BoneTransforms()
    {
      return boneNames.Select(name => customSkeleton.GetBoneFromBoneName(name))
        .Where(bone => bone != null && bone.alwaysUpdatesTransform != null).Select(bone => bone.alwaysUpdatesTransform)
        .ToArray();
    }

    private float AbsoluteStraightness(Transform[] boneTransforms)
    {
      var length = Length(boneTransforms);
      var distance = Vector3.Distance(
        boneTransforms.First().position,
        boneTransforms.Last().position);

      var absoluteStraightness = distance / length;
      return absoluteStraightness;
    }
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
}

[Serializable]
public class SendMessageDisplay
{
  [HideInInspector]
  public int size = 5;
  public string[] messages = new string[] { };

  public void AddMessage(string message, Transform transform)
  {
    messages = messages.Prepend(message).ToArray();
    messages = messages.Take(size).ToArray();

    var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    primitive.transform.position = transform.position;
    primitive.transform.localScale = new Vector3(0.01F, 0.01F, 0.01F);

    GameObject.Destroy(primitive, 1);
  }
}