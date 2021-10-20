using UnityEngine;

public class ArmBehavior : MonoBehaviour
{
  public ArmBehaviorType behavior = ArmBehaviorType.None;
  public Handedness.HandTypes handType = Handedness.HandTypes.HandLeft;

  private ArmBehaviorType _behavior = ArmBehaviorType.None;

  public enum ArmBehaviorType
  {
    None,
    User,
    Static,
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

        // Colliders
        gameObject.RemoveComponent<HandCollider>();
        gameObject.RemoveComponent<ForearmCollider>();
        gameObject.RemoveComponent<HumerusCollider>();

        // Mixers for different sources of movement
        gameObject.RemoveComponent<ApplyPose>();
        gameObject.RemoveComponent<ApplyRootTracking>();
        gameObject.RemoveComponent<ApplyHandTracking>();
        break;
      case ArmBehaviorType.Static:
        // Player movement algorithms
        gameObject.RemoveComponent<InverseKinematics>();
        gameObject.RemoveComponent<CustomHand>();
        gameObject.RemoveComponent<Skeleton>();
        gameObject.RemoveComponent<DisableArmOnUntracked>();

        // Grabbing state machine
        gameObject.AddIfNotExisting<Idle>();
        gameObject.RemoveComponent<Grabbed>();
        gameObject.RemoveComponent<Grabbing>();

        // Hand gestures
        gameObject.RemoveComponent<GestureState>();
        gameObject.RemoveComponent<PinchState>();

        // Colliders
        gameObject.AddIfNotExisting<HandCollider>();
        gameObject.AddIfNotExisting<ForearmCollider>();
        gameObject.AddIfNotExisting<HumerusCollider>();

        // Mixers for different sources of movement
        gameObject.RemoveComponent<ApplyPose>();
        gameObject.RemoveComponent<ApplyRootTracking>();
        gameObject.RemoveComponent<ApplyHandTracking>();
        break;

      case ArmBehaviorType.User:
        // Player movement algorithms
        gameObject.AddIfNotExisting<InverseKinematics>();
        gameObject.AddIfNotExisting<CustomHand>();
        gameObject.AddIfNotExisting<Skeleton>();
        gameObject.AddIfNotExisting<DisableArmOnUntracked>();

        // Grabbing state machine
        gameObject.AddIfNotExisting<Idle>();
        gameObject.RemoveComponent<Grabbed>();
        gameObject.RemoveComponent<Grabbing>();

        // Hand gestures
        gameObject.AddIfNotExisting<GestureState>();
        gameObject.AddIfNotExisting<PinchState>();

        // Colliders
        gameObject.AddIfNotExisting<HandCollider>();
        gameObject.AddIfNotExisting<ForearmCollider>();
        gameObject.AddIfNotExisting<HumerusCollider>();

        // Mixers for different sources of movement
        gameObject.AddIfNotExisting<ApplyPose>();
        gameObject.AddIfNotExisting<ApplyRootTracking>();
        gameObject.AddIfNotExisting<ApplyHandTracking>();
        break;
      default:
        break;

    }
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

