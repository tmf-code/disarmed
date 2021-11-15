using UnityEngine;

public class ArmBehaviour : MonoBehaviour
{
  private ArmOwnerType _owner = ArmOwnerType.User;
  public ArmOwnerType owner = ArmOwnerType.User;

  public ArmBehaviorType behavior = ArmBehaviorType.None;
  private ArmBehaviorType _behavior = ArmBehaviorType.None;

  public enum ArmBehaviorType
  {
    None,
    TrackUserInputNoGrab,
    TrackUserInput,
    Grabbed,
    Ragdoll,
    CopyArmMovement,
    ResponsiveRagdoll,
    MovementPlaybackRagdoll,
    MovementPlayback,
    MovementPlaybackArmSocket,
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

    switch (_behavior)
    {
      case ArmBehaviorType.None:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

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
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

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
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

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

      case ArmBehaviorType.TrackUserInputNoGrab:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();

          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();


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

      case ArmBehaviorType.TrackUserInput:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }


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

      case ArmBehaviorType.CopyArmMovement:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();


          // Grabbing state machine

          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();



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
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();


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
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.AddIfNotExisting<ApplyRecordedMovementRagdoll>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }


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

      case ArmBehaviorType.MovementPlayback:
        {
          gameObject.AddIfNotExisting<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();


          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
            gameObject.RemoveComponent<Grabbing>();
          }

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

      case ArmBehaviorType.MovementPlaybackArmSocket:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.AddIfNotExisting<ApplyRecordedShoulder>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();
          gameObject.RemoveComponent<Grabbing>();

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
    if (_behavior != behavior || _owner != owner)
    {
      _behavior = behavior;
      _owner = owner;
      UpdateBehavior();
    }
  }
}

