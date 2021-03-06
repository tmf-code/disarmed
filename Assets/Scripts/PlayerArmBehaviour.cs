using UnityEngine;

public class PlayerArmBehaviour : MonoBehaviour
{
  public PlayerArmBehaviours behaviour = PlayerArmBehaviours.None;
  private PlayerArmBehaviours _behaviour = PlayerArmBehaviours.None;

  public enum PlayerArmBehaviours
  {
    None,
    TrackUserInput,
    TrackUserCollideWithArms,
    ResponsiveRagdoll,
    MovementPlaybackArmSocket,
    TrackUserInputGrabSelf,
  }

  public void UpdateBehaviour(PlayerArmBehaviours behaviour)
  {
    this.behaviour = behaviour;
    _behaviour = behaviour;

    switch (_behaviour)
    {
      case PlayerArmBehaviours.None:
        {
          gameObject.RemoveComponent<PlayerCollides>();
          gameObject.AddIfNotExisting<PlayerInteracts>();
          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();

          break;
        }

      case PlayerArmBehaviours.TrackUserInput:
        {
          GrabOperations.AddAbilities(gameObject, true, false);

          gameObject.RemoveComponent<PlayerCollides>();
          gameObject.AddIfNotExisting<PlayerInteracts>();
          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();

          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          break;
        }

      case PlayerArmBehaviours.TrackUserCollideWithArms:
        {
          GrabOperations.AddAbilities(gameObject, true, false);

          gameObject.AddIfNotExisting<PlayerCollides>();
          gameObject.RemoveComponent<PlayerInteracts>();

          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();

          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          break;
        }

      case PlayerArmBehaviours.TrackUserInputGrabSelf:
        {
          GrabOperations.AddAbilities(gameObject, true, true);

          gameObject.AddIfNotExisting<PlayerCollides>();
          gameObject.RemoveComponent<PlayerInteracts>();

          gameObject.AddIfNotExisting<ApplyInverseKinematics>();
          gameObject.AddIfNotExisting<ApplyRootTracking>();
          gameObject.AddIfNotExisting<ApplyHandTracking>();

          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          break;
        }

      case PlayerArmBehaviours.ResponsiveRagdoll:
        {
          GrabOperations.AddAbilities(gameObject, false, false);

          gameObject.RemoveComponent<PlayerCollides>();
          gameObject.RemoveComponent<PlayerInteracts>();

          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.AddIfNotExisting<ApplyVRTrackingDataToModelRagdoll>();

          gameObject.RemoveComponent<ApplyRecordedShoulder>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();

          break;
        }

      case PlayerArmBehaviours.MovementPlaybackArmSocket:
        {
          GrabOperations.AddAbilities(gameObject, false, false);

          gameObject.AddIfNotExisting<ApplyRecordedShoulder>();

          gameObject.RemoveComponent<PlayerCollides>();
          gameObject.AddIfNotExisting<PlayerInteracts>();

          gameObject.RemoveComponent<ApplyVRTrackingDataToModelRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<ApplyInverseKinematics>();
          gameObject.RemoveComponent<ApplyRootTracking>();
          gameObject.RemoveComponent<ApplyHandTracking>();
          break;
        }
      default:
        break;
    }
  }

  private void Update()
  {
    if (_behaviour != behaviour)
    {
      UpdateBehaviour(behaviour);
    }
  }
}