using UnityEngine;

public class PlayerArmBehaviour : MonoBehaviour
{
  public PlayerArmBehaviours behaviour = PlayerArmBehaviours.None;
  private PlayerArmBehaviours _behaviour = PlayerArmBehaviours.None;

  public enum PlayerArmBehaviours
  {
    None,
    TrackUserInputNoGrab,
    TrackUserInput,
    ResponsiveRagdoll,
    MovementPlaybackArmSocket,
  }

  void OnDestroy()
  {
    UpdateBehaviour(PlayerArmBehaviours.None);
  }

  private void UpdateBehaviour(PlayerArmBehaviours behaviour)
  {
    _behaviour = behaviour;
    switch (_behaviour)
    {
      case PlayerArmBehaviours.None:
        {
          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbing>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();

          break;
        }

      case PlayerArmBehaviours.TrackUserInputNoGrab:
        {
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();

          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbing>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<ApplyPose>();

          break;
        }

      case PlayerArmBehaviours.TrackUserInput:
        {
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyPose>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbing>();
          }

          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          break;
        }

      case PlayerArmBehaviours.ResponsiveRagdoll:
        {
          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.AddIfNotExisting<ApplyVRTrackingDataToModelRagdoll>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbing>();


          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();

          break;
        }

      case PlayerArmBehaviours.MovementPlaybackArmSocket:
        {
          gameObject.AddIfNotExisting<ApplyRecordedShoulder>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbing>();

          // Mixers for different sources of movement to be applied to model
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyPose>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          break;
        }
      default:
        break;
    }
  }

  private bool HasAnyGrabbingState()
  {
    return gameObject.HasComponent<Idle>()
      || gameObject.HasComponent<Grabbing>();
  }

  private void Update()
  {
    if (_behaviour != behaviour)
    {
      UpdateBehaviour(behaviour);
    }
  }
}