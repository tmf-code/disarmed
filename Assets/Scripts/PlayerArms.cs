using UnityEngine;
using static PlayerArmBehaviour;
using static WorldArmBehaviour;

public class PlayerArms : MonoBehaviour
{
  public PlayerArmBehaviour left;
  public PlayerArmBehaviour right;

  public void RemoveLeft(WorldArmBehaviours nextBehaviour)
  {
    if (left != null)
    {
      left.transform.parent = null;
      var arm = left.gameObject;
      arm.RemoveComponent<PlayerArmBehaviour>();
      var worldArmBehaviour = arm.AddIfNotExisting<WorldArmBehaviour>();
      worldArmBehaviour.behaviour = nextBehaviour;
      left = null;
    }
  }

  public void RemoveRight(WorldArmBehaviours nextBehaviour)
  {
    if (right != null)
    {
      right.transform.parent = null;
      var arm = right.gameObject;
      arm.RemoveComponent<PlayerArmBehaviour>();
      var worldArmBehaviour = arm.AddIfNotExisting<WorldArmBehaviour>();
      worldArmBehaviour.behaviour = nextBehaviour;
      right = null;
    }
  }

  public void RemoveArm(PlayerArmBehaviour arm, WorldArmBehaviours nextBehaviour)
  {
    if (arm != left && arm != right)
    {
      throw new System.Exception("Could not remove arm from player. Arm was not on player");
    }

    if (arm.gameObject.GetComponentOrThrow<Handedness>().IsLeft())
    {
      RemoveLeft(nextBehaviour);
    }
    else
    {
      RemoveRight(nextBehaviour);
    }
  }

  public bool HasLeft()
  {
    return left != null;
  }
  public bool HasRight()
  {
    return right != null;
  }

  public bool HasHand(Handedness.HandTypes hand)
  {
    if (hand == Handedness.HandTypes.HandLeft)
    {
      return HasLeft();
    }
    else
    {
      return HasRight();
    }
  }

  public void AddArm(WorldArmBehaviour arm, PlayerArmBehaviours nextBehaviour)
  {
    if (arm.gameObject.GetComponentOrThrow<Handedness>().IsLeft())
    {
      AddLeft(arm, nextBehaviour);
    }
    else
    {
      AddRight(arm, nextBehaviour);
    }
  }

  public void AddLeft(WorldArmBehaviour arm, PlayerArmBehaviours nextBehaviour)
  {
    if (left != null) throw new System.Exception("Could not add arm to left slot. Something already there");
    var handType = arm.gameObject.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandLeft) throw new System.Exception("Can not add right arm to left slot");

    var gameObject = arm.gameObject;
    Destroy(arm);

    var playerArmBehaviour = gameObject.AddIfNotExisting<PlayerArmBehaviour>();
    playerArmBehaviour.behaviour = nextBehaviour;

    var pivot = gameObject.GetComponentOrThrow<PivotPoint>();
    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    left = playerArmBehaviour;
  }

  public void AddRight(WorldArmBehaviour arm, PlayerArmBehaviours nextBehaviour)
  {
    if (right != null) throw new System.Exception("Could not add arm to right slot. Something already there");
    var handType = arm.gameObject.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandRight) throw new System.Exception("Can not add right arm to right slot");

    var gameObject = arm.gameObject;
    Destroy(arm);

    var playerArmBehaviour = gameObject.AddIfNotExisting<PlayerArmBehaviour>();
    playerArmBehaviour.behaviour = nextBehaviour;

    var pivot = gameObject.GetComponentOrThrow<PivotPoint>();
    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    right = playerArmBehaviour;
  }
}
