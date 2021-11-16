using UnityEngine;

public class WorldArmBehaviour : MonoBehaviour
{
  public WorldArmBehaviours behaviour = WorldArmBehaviours.None;
  private WorldArmBehaviours _behaviour = WorldArmBehaviours.None;

  public enum WorldArmBehaviours
  {
    None,
    Grabbed,
    Ragdoll,
    CopyArmMovement,
    MovementPlaybackRagdoll,
    MovementPlayback,
  }

  void OnDestroy()
  {
    UpdateBehaviour(WorldArmBehaviours.None);
  }

  private void UpdateBehaviour(WorldArmBehaviours behaviour)
  {
    _behaviour = behaviour;

    switch (_behaviour)
    {
      case WorldArmBehaviours.None:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();

          break;
        }
      case WorldArmBehaviours.Grabbed:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();

          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
          }
          break;
        }

      case WorldArmBehaviours.Ragdoll:
        {
          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<CopyArmMovement>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
          }

          break;
        }

      case WorldArmBehaviours.CopyArmMovement:
        {
          gameObject.AddIfNotExisting<CopyArmMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();

          // Grabbing state machine
          gameObject.RemoveComponent<Idle>();
          gameObject.RemoveComponent<Grabbed>();

          break;
        }

      case WorldArmBehaviours.MovementPlaybackRagdoll:
        {
          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.AddIfNotExisting<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<CopyArmMovement>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
          }

          break;
        }

      case WorldArmBehaviours.MovementPlayback:
        {
          gameObject.AddIfNotExisting<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();

          // Grabbing state machine
          if (!HasAnyGrabbingState())
          {
            gameObject.AddIfNotExisting<Idle>();
            gameObject.RemoveComponent<Grabbed>();
          }

          break;
        }

      default:
        break;

    }
  }

  private bool HasAnyGrabbingState()
  {
    return gameObject.HasComponent<Idle>()
      || gameObject.HasComponent<Grabbed>();
  }

  private void Update()
  {
    if (_behaviour != behaviour)
    {
      UpdateBehaviour(behaviour);
    }
  }
}

