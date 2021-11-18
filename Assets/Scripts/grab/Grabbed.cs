using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Grabbed : MonoBehaviour, PairedAction
{
  public Grabbing other;
  PairedAction PairedAction.other { get => other; }

  private new SimpleAnimation animation;
  public Quaternion selectedStrategy;
  static readonly List<Quaternion> strategies;

  static Grabbed()
  {
    var quarterTurnY = Quaternion.AngleAxis(90, new Vector3(0, 1, 0));
    var antiQuarterTurnY = Quaternion.AngleAxis(-90, new Vector3(0, 1, 0));
    var halfTurnZ = Quaternion.AngleAxis(180, new Vector3(0, 0, 1));

    strategies = new List<Quaternion> {
      quarterTurnY,
      antiQuarterTurnY,
      halfTurnZ * quarterTurnY,
      halfTurnZ * antiQuarterTurnY,
    };
  }

  void Start()
  {
    animation = new SimpleAnimation(3, SimpleAnimation.EasingFunction.Linear, Time.time);
  }

  public static Quaternion GetStrategy(GameObject current, GameObject other)
  {
    var grabRotation = other.transform.rotation;
    var grabRotations = strategies.ConvertAll((rotation) =>
      grabRotation * rotation);

    var angularDistances = grabRotations.ConvertAll((targetRotation) =>
      Quaternion.Angle(current.transform.FindRecursiveOrThrow("Model").rotation, targetRotation)
    );

    var sortedDistances = angularDistances
      .Select((value, index) => new Tuple<float, int>(value, index))
      .OrderBy((item) => item.Item1);
    var selectedStrategyIndex = sortedDistances.First().Item2;
    return strategies[selectedStrategyIndex];
  }

  void Update()
  {
    var targetTransform = GetTargetTransform(other.gameObject);
    animation.Update(Time.time);

    var currentTransform = transform.FindRecursiveOrThrow("Model");

    var position = Vector3.LerpUnclamped(
      currentTransform.position,
      targetTransform.position,
      animation.progression);

    var rotation = Quaternion.SlerpUnclamped(
      currentTransform.rotation,
      targetTransform.rotation * selectedStrategy,
      animation.progression);

    currentTransform.SetPositionAndRotation(position, rotation);
  }

  public static Transform GetTargetTransform(GameObject other)
  {
    var grabbingHandPrefix = other.GetComponentOrThrow<Handedness>().HandPrefix();
    var targetTransform = other.transform.FindChildRecursive($"{grabbingHandPrefix}_palm_center_marker");
    return targetTransform;
  }
}

