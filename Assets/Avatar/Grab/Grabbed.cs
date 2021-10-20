using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grabbed : MonoBehaviour
{
  public Grabbing grabbing;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  private new SimpleAnimation animation;
  private Quaternion selectedStrategy;
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
    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;


    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbing>();

    animation = new SimpleAnimation(3);

    var grabRotation = GetTargetTransform().rotation;
    var grabRotations = strategies.ConvertAll((rotation) =>
      grabRotation * rotation);

    var angularDistances = grabRotations.ConvertAll((targetRotation) =>
      Quaternion.Angle(transform.FindRecursiveOrThrow("Model").rotation, targetRotation)
    );

    var sortedDistances = angularDistances
      .Select((value, index) => new Tuple<float, int>(value, index))
      .OrderBy((item) => item.Item1);
    var selectedStrategyIndex = sortedDistances.First().Item2;
    selectedStrategy = strategies[selectedStrategyIndex];
  }

  void Update()
  {
    animation.Update();
    var targetTransform = GetTargetTransform();
    var currentTransform = transform.FindRecursiveOrThrow("Model");

    currentTransform.SetPositionAndRotation(
      Vector3.Lerp(
        currentTransform.position,
        targetTransform.position,
        animation.progression
      ),

      Quaternion.Slerp(
        currentTransform.rotation,
        targetTransform.rotation * selectedStrategy,
        animation.progression
      )
    );

    var currentTime = Time.time - creationTime;
    if (currentTime > minimumIdleTimeSeconds) canTransition = true;
    else canTransition = false;
  }

  private Transform GetTargetTransform()
  {
    var grabbingHandPrefix = grabbing.gameObject.GetComponentOrThrow<Handedness>().HandPrefix();
    var targetTransform = grabbing.transform.FindRecursiveOrThrow($"{grabbingHandPrefix}_palm_center_marker");
    return targetTransform;
  }

  public void OnGrabReleased()
  {
    if (!canTransition) return;

    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;

    gameObject.AddIfNotExisting<Idle>();

    Destroy(this);
  }

  [Serializable]
  class SimpleAnimation
  {
    public float timeCreated = Time.time;
    public float duration = 1;
    public float lifetime = 0;
    public float progression = 0;

    public SimpleAnimation(float duration)
    {
      this.duration = duration;
    }

    public float Update()
    {
      lifetime = Time.time - timeCreated;
      progression = Mathf.Clamp(lifetime / duration, 0, 1);

      return progression;
    }
  }

}

