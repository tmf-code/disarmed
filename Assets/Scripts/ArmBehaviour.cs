using UnityEngine;

public class ArmBehaviour : MonoBehaviour
{
  public ArmBehaviorType behavior = ArmBehaviorType.None;
  public Handedness.HandTypes handType = Handedness.HandTypes.HandLeft;
  public ArmOwnerType owner = ArmOwnerType.User;

  private ArmBehaviorType _behavior = ArmBehaviorType.None;

  public enum ArmBehaviorType
  {
    None,
    TrackUserInput,
    Grabbed,
    Ragdoll,
    CopyArmMovement,
  }

  public enum ArmOwnerType
  {
    User,
    World,
  }

  private void Start()
  {
    UpdateBehavior();
  }

  private void UpdateBehavior()
  {
    var handedness = gameObject.AddIfNotExisting<Handedness>();
    handedness.handType = handType;

    switch (_behavior)
    {
      case ArmBehaviorType.None:
        {
          // Player movement algorithms
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }
      case ArmBehaviorType.Grabbed:
        {
          // Player movement algorithms
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();


          // Mixers for different sources of movement
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case ArmBehaviorType.Ragdoll:
        {
          // Player movement algorithms
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.AddIfNotExisting<RagDollArm>();

          // Mixers for different sources of movement
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case ArmBehaviorType.TrackUserInput:
        {
          // Player movement algorithms
          gameObject.AddIfNotExisting<InverseKinematics>();
          gameObject.AddIfNotExisting<CustomHand>();
          gameObject.AddIfNotExisting<Skeleton>();
          gameObject.AddIfNotExisting<DisableArmOnUntracked>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

          // Hand gestures
          gameObject.AddIfNotExisting<GestureState>();
          gameObject.AddIfNotExisting<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement
          gameObject.AddIfNotExisting<ApplyPose>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();
          break;
        }

      case ArmBehaviorType.CopyArmMovement:
        {
          // Player movement algorithms
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine

          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();


          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.AddIfNotExisting<CopyArmMovement>();
          break;
        }
      default:
        break;

    }
  }

  private bool HasAnyGrabbingState()
  {
    return gameObject.HasComponent<Idle>()
      || gameObject.HasComponent<Grabbed>()
      || gameObject.HasComponent<Grabbing>();
  }

  private void Update()
  {
    if (_behavior != behavior)
    {
      _behavior = behavior;
      UpdateBehavior();
    }
  }
}

