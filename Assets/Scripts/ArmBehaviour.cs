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
    SwapArms,
    Grabbed,
    Ragdoll,
    CopyArmMovement,
    ResponsiveRagdoll,
    MovementPlaybackRagdoll,
    MovementPlayback,
  }

  public enum ArmOwnerType
  {
    User,
    World,
  }

  private void Awake()
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
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<ApplyRecordedMovementToVRTrackingData>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }
      case ArmBehaviorType.Grabbed:
        {
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<ApplyRecordedMovementToVRTrackingData>();
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


          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case ArmBehaviorType.Ragdoll:
        {
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<ApplyRecordedMovementToVRTrackingData>();
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

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case ArmBehaviorType.TrackUserInput:
        {
          // Arm movement sources
          gameObject.AddIfNotExisting<InverseKinematics>();
          gameObject.AddIfNotExisting<CustomHand>();
          var skel = gameObject.AddIfNotExisting<Skeleton>();
          skel.isSwapped = false;
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

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyPose>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();
          break;
        }

      case ArmBehaviorType.SwapArms:
        {
          // Arm movement sources
          gameObject.AddIfNotExisting<InverseKinematics>();
          gameObject.AddIfNotExisting<CustomHand>();
          var skel = gameObject.AddIfNotExisting<Skeleton>();
          skel.isSwapped = true;


          gameObject.AddIfNotExisting<DisableArmOnUntracked>();


          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();
          break;
        }

      case ArmBehaviorType.CopyArmMovement:
        {
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.RemoveComponent<ApplyRecordedMovementToVRTrackingData>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine

          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();


          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.RemoveComponent<RagDollArm>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.AddIfNotExisting<CopyArmMovement>();
          break;
        }

      case ArmBehaviorType.ResponsiveRagdoll:
        {
          // Arm movement sources
          gameObject.AddIfNotExisting<InverseKinematics>();
          gameObject.AddIfNotExisting<CustomHand>();
          gameObject.AddIfNotExisting<Skeleton>();
          gameObject.RemoveComponent<DisableArmOnUntracked>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

          // Hand gestures
          gameObject.RemoveComponent<GestureState>();
          gameObject.RemoveComponent<PinchState>();

          gameObject.AddIfNotExisting<RagDollArm>();

          // Mixers for different sources of movement to be applied to model
          gameObject.AddIfNotExisting<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case ArmBehaviorType.MovementPlaybackRagdoll:
        {
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.AddIfNotExisting<ApplyRecordedMovementToVRTrackingData>();
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

          // Mixers for different sources of movement to be applied to model
          gameObject.AddIfNotExisting<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();
          break;
        }

      case ArmBehaviorType.MovementPlayback:
        {
          // Arm movement sources
          gameObject.RemoveComponent<InverseKinematics>();
          gameObject.RemoveComponent<CustomHand>();
          gameObject.RemoveComponent<Skeleton>();
          gameObject.AddIfNotExisting<ApplyRecordedMovementToVRTrackingData>();
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

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();
          gameObject.RemoveComponent<CopyArmMovement>();
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

