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

  public void UpdateBehaviour(WorldArmBehaviours behaviour)
  {
    this.behaviour = behaviour;
    _behaviour = behaviour;

    switch (_behaviour)
    {
      case WorldArmBehaviours.None:
        {
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();
          break;
        }

      case WorldArmBehaviours.Grabbed:
        {
          // Don't mess with the grabbing

          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();

          break;
        }

      case WorldArmBehaviours.Ragdoll:
        {
          GrabOperations.AddAbilities(gameObject, false, true);

          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<CopyArmMovement>();


          break;
        }

      case WorldArmBehaviours.CopyArmMovement:
        {
          GrabOperations.AddAbilities(gameObject, false, true);

          gameObject.AddIfNotExisting<CopyArmMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();


          break;
        }

      case WorldArmBehaviours.MovementPlaybackRagdoll:
        {
          GrabOperations.AddAbilities(gameObject, false, true);

          gameObject.AddIfNotExisting<RagDollArm>();
          gameObject.AddIfNotExisting<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<ApplyRecordedMovement>();
          gameObject.RemoveComponent<CopyArmMovement>();


          break;
        }

      case WorldArmBehaviours.MovementPlayback:
        {
          GrabOperations.AddAbilities(gameObject, false, true);

          gameObject.AddIfNotExisting<ApplyRecordedMovement>();
          gameObject.RemoveComponent<ApplyRecordedMovementRagdoll>();
          gameObject.RemoveComponent<RagDollArm>();
          gameObject.RemoveComponent<CopyArmMovement>();

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

