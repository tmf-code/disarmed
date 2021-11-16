using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerArmBehaviour;
using static WorldArmBehaviour;

public partial class Grabbed : MonoBehaviour
{
  public Grabbing grabbing;

  [SerializeField] [HideInInspector] private new SimpleAnimation animation;
  private Option<Quaternion> selectedStrategy = new None<Quaternion>();
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
    gameObject.GetComponentOrThrow<WorldArmBehaviour>().behaviour = WorldArmBehaviours.Grabbed;
    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbing>();

    animation = new SimpleAnimation(3, SimpleAnimation.EasingFunction.Linear, Time.time);

    selectedStrategy = selectedStrategy.IsNone() ? GetStrategy() : selectedStrategy;
  }

  private Option<Quaternion> GetStrategy()
  {
    var grabRotation = GetTargetTransform(grabbing).Unwrap().rotation;
    var grabRotations = strategies.ConvertAll((rotation) =>
      grabRotation * rotation);

    var angularDistances = grabRotations.ConvertAll((targetRotation) =>
      Quaternion.Angle(transform.FindRecursiveOrThrow("Model").rotation, targetRotation)
    );

    var sortedDistances = angularDistances
      .Select((value, index) => new Tuple<float, int>(value, index))
      .OrderBy((item) => item.Item1);
    var selectedStrategyIndex = sortedDistances.First().Item2;
    return Option<Quaternion>.of(strategies[selectedStrategyIndex]);
  }

  void Update()
  {
    selectedStrategy = selectedStrategy.IsNone() ? GetStrategy() : selectedStrategy;
    var targetTransform = GetTargetTransform(grabbing);

    if (!selectedStrategy.TryUnwrap(out var strategy)) return;
    if (!targetTransform.TryUnwrap(out var target)) return;
    animation.Update(Time.time);

    var currentTransform = transform.FindRecursiveOrThrow("Model");

    var position = Vector3.LerpUnclamped(
      currentTransform.position,
      target.position,
      animation.progression);

    var rotation = Quaternion.SlerpUnclamped(
      currentTransform.rotation,
      target.rotation * strategy,
      animation.progression);

    currentTransform.SetPositionAndRotation(position, rotation);
  }

  private Option<Transform> GetTargetTransform(Grabbing grabbing)
  {
    if (grabbing == null) return new None<Transform>();
    var grabbingHandPrefix = grabbing.gameObject.GetComponentOrThrow<Handedness>().HandPrefix();
    var targetTransform = grabbing.transform.FindChildRecursive($"{grabbingHandPrefix}_palm_center_marker");
    return Option<Transform>.of(targetTransform);
  }

  void OnDestroy()
  {
    var maybeTimeline = GameObject.Find("Timeline");
    WorldArmBehaviours nextBehaviour;
    if (maybeTimeline && maybeTimeline.GetComponent<Timeline>().act > Timeline.Acts.Four)
    {
      nextBehaviour = WorldArmBehaviours.MovementPlaybackRagdoll;
    }
    else
    {
      nextBehaviour = WorldArmBehaviours.Ragdoll;
    }

    var armBehavior = gameObject.GetComponentOrThrow<WorldArmBehaviour>();
    armBehavior.behaviour = nextBehaviour;
    Option<Grabbing>.of(grabbing).End(grabbed => grabbed.OnDestroy());
  }

  public void OnGrabReleased()
  {
    Destroy(this);
  }

  public void CollideWithShoulder(Collider shoulderCollider)
  {

    var handedness = gameObject.GetComponentOrThrow<Handedness>();
    // This arm should be a world arm
    if (gameObject.TryGetComponent<WorldArmBehaviour>(out var behaviour)) return;

    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();

    var thisHandIsLeft = handedness.IsLeft();
    var tag = thisHandIsLeft ? "LeftShoulder" : "RightShoulder";
    var isCorrectShoulder = shoulderCollider.CompareTag(tag);

    if (!isCorrectShoulder) return;

    var spotIsFree = !playerArms.HasHand(handedness.handType);

    if (!spotIsFree) return;

    AttachArm(behaviour);
  }

  private void AttachArm(WorldArmBehaviour behaviour)
  {
    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    playerArms.AddArm(behaviour, PlayerArmBehaviours.TrackUserInput);
    grabbing.OnArmAttach();
    Destroy(this);
  }
}

