using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Grabbed : MonoBehaviour
{
  public Grabbing grabbing;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  [SerializeField] [HideInInspector] private new SimpleAnimation animation;
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
    gameObject.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.Grabbed;
    gameObject.GetOptionComponent<ApplyInverseKinematics>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);

    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbing>();

    animation = new SimpleAnimation(3, SimpleAnimation.EasingFunction.Linear, Time.time);

    var grabRotation = GetTargetTransform().Unwrap().rotation;
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
    animation.Update(Time.time);
    var targetTransform = GetTargetTransform();
    var currentTransform = transform.FindRecursiveOrThrow("Model");

    targetTransform.Match(
      none: () => OnGrabReleased(),
      some: targetTransform =>
      {
        currentTransform.SetPositionAndRotation(
          Vector3.LerpUnclamped(
            currentTransform.position,
            targetTransform.position,
            animation.progression
          ),

          Quaternion.SlerpUnclamped(
            currentTransform.rotation,
            targetTransform.rotation * selectedStrategy,
            animation.progression
          )
        );

        var currentTime = Time.time - creationTime;
        if (currentTime > minimumIdleTimeSeconds) canTransition = true;
        else canTransition = false;
      }
    );
  }

  private Option<Transform> GetTargetTransform()
  {
    if (grabbing == null) return new None<Transform>();
    var grabbingHandPrefix = grabbing.gameObject.GetComponentOrThrow<Handedness>().HandPrefix();
    var targetTransform = grabbing.transform.FindChildRecursive($"{grabbingHandPrefix}_palm_center_marker");
    return Option<Transform>.of(targetTransform);
  }

  public void OnGrabReleased()
  {
    if (!canTransition) return;

    var armBehavior = gameObject.GetComponentOrThrow<ArmBehaviour>();
    var isUserArm = armBehavior.owner == ArmBehaviour.ArmOwnerType.User;

    if (isUserArm)
    {
      var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
      playerArms.RemoveArm(gameObject);
    }

    armBehavior.owner = ArmBehaviour.ArmOwnerType.World;

    var maybeTimeline = GameObject.Find("Timeline");
    if (maybeTimeline && maybeTimeline.GetComponent<Timeline>().act > Timeline.Acts.Four)
    {
      armBehavior.behavior = ArmBehaviour.ArmBehaviorType.MovementPlaybackRagdoll;
    }
    else
    {
      armBehavior.behavior = ArmBehaviour.ArmBehaviorType.Ragdoll;
    }

    Destroy(this);
    gameObject.AddIfNotExisting<Idle>();
  }

  private void RepositionAndClone()
  {
    gameObject.GetOptionComponent<ApplyInverseKinematics>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);
    gameObject.GetComponent<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.TrackUserInput;

    var clone = Instantiate(gameObject);
    clone.GetComponent<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.Ragdoll;
    clone.GetComponent<ArmBehaviour>().owner = ArmBehaviour.ArmOwnerType.World;
  }

  public void CollideWithShoulder(Collider shoulderCollider)
  {

    var handedness = gameObject.GetComponentOrThrow<Handedness>();
    var armBehaviour = gameObject.GetComponentOrThrow<ArmBehaviour>();
    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();

    var thisHandIsLeft = handedness.IsLeft();
    var tag = thisHandIsLeft ? "LeftShoulder" : "RightShoulder";
    var isCorrectShoulder = shoulderCollider.CompareTag(tag);

    if (!isCorrectShoulder) return;

    var isWorldControlled = armBehaviour.owner == ArmBehaviour.ArmOwnerType.World;
    if (!isWorldControlled) return;

    var spotIsFree = !playerArms.HasHand(handedness.handType);

    if (!spotIsFree) return;

    AttachArm();
  }

  private void AttachArm()
  {
    var armBehaviour = gameObject.GetComponentOrThrow<ArmBehaviour>();
    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    armBehaviour.behavior = ArmBehaviour.ArmBehaviorType.TrackUserInput;
    armBehaviour.owner = ArmBehaviour.ArmOwnerType.User;
    playerArms.AddArm(gameObject);
    grabbing.OnArmAttach();

    gameObject.GetOptionComponent<ApplyInverseKinematics>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyHandTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyRootTracking>().Map(component => component.strength = 1);
    gameObject.GetOptionComponent<ApplyPose>().Map(component => component.strength = 0);
    gameObject.GetOptionComponent<PivotPoint>().Map(component => component.pivotPointType = PivotPoint.PivotPointType.Wrist);
    gameObject.AddIfNotExisting<Idle>();
    Destroy(this);
  }
}

