using System;
using UnityEngine;
using static PlayerArmBehaviour;

public class PlayerArms : MonoBehaviour
{
  public PlayerArmBehaviour left;
  public PlayerArmBehaviour right;

  public void RemoveLeft()
  {
    if (left != null)
    {
      left.transform.parent = null;
      var arm = left.gameObject;
      left.UpdateBehaviour(PlayerArmBehaviours.None);
      arm.RemoveComponent<PlayerArmBehaviour>();
      arm.AddIfNotExisting<WorldArmBehaviour>();
      left = null;
    }
  }

  public void RemoveRight()
  {
    if (right != null)
    {
      right.transform.parent = null;
      var arm = right.gameObject;
      right.UpdateBehaviour(PlayerArmBehaviours.None);
      arm.RemoveComponent<PlayerArmBehaviour>();
      arm.AddIfNotExisting<WorldArmBehaviour>();
      right = null;
    }
  }

  public void RemoveArm(PlayerArmBehaviour arm)
  {
    if (arm != left && arm != right)
    {
      throw new System.Exception("Could not remove arm from player. Arm was not on player");
    }

    if (arm.gameObject.GetComponentOrThrow<Handedness>().IsLeft())
    {
      RemoveLeft();
    }
    else
    {
      RemoveRight();
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

  public void AddArm(WorldArmBehaviour arm)
  {
    if (arm.gameObject.GetComponentOrThrow<Handedness>().IsLeft())
    {
      AddLeft(arm);
    }
    else
    {
      AddRight(arm);
    }
  }

  public void AddLeft(WorldArmBehaviour arm)
  {
    if (left != null) throw new Exception("Could not add arm to left slot. Something already there");
    var handType = arm.gameObject.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandLeft) throw new Exception("Can not add right arm to left slot");

    var gameObject = arm.gameObject;
    Destroy(arm);
    var playerArmBehaviour = gameObject.AddIfNotExisting<PlayerArmBehaviour>();
    playerArmBehaviour.UpdateBehaviour(PlayerArmBehaviours.TrackUserInputGrabSelf);

    var pivot = gameObject.GetComponentOrThrow<PivotPoint>();
    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    left = playerArmBehaviour;
  }

  public void AddRight(WorldArmBehaviour arm)
  {
    if (right != null) throw new Exception("Could not add arm to right slot. Something already there");
    var handType = arm.gameObject.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandRight) throw new Exception("Can not add right arm to right slot");

    var gameObject = arm.gameObject;
    Destroy(arm);
    var playerArmBehaviour = gameObject.AddIfNotExisting<PlayerArmBehaviour>();
    playerArmBehaviour.UpdateBehaviour(PlayerArmBehaviours.TrackUserInputGrabSelf);

    var pivot = gameObject.GetComponentOrThrow<PivotPoint>();
    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    right = playerArmBehaviour;
  }
}
