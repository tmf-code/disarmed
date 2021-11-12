using UnityEngine;

public class PlayerArms : MonoBehaviour
{
  public GameObject left;
  public GameObject right;

  public void RemoveLeft()
  {
    if (left != null)
    {
      left.transform.parent = null;
      left.GetComponentOrThrow<ArmBehaviour>().owner = ArmBehaviour.ArmOwnerType.World;
      left = null;
    }
  }

  public void RemoveRight()
  {
    if (right != null)
    {
      right.transform.parent = null;
      right.GetComponentOrThrow<ArmBehaviour>().owner = ArmBehaviour.ArmOwnerType.World;
      right = null;
    }
  }

  public void RemoveArm(GameObject arm)
  {
    if (arm != left && arm != right)
    {
      throw new System.Exception("Could not remove arm from player. Arm was not on player");
    }

    if (arm.GetComponentOrThrow<Handedness>().IsLeft())
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

  public void AddArm(GameObject arm)
  {
    if (arm.GetComponentOrThrow<Handedness>().IsLeft())
    {
      AddLeft(arm);
    }
    else
    {
      AddRight(arm);
    }
  }

  public void AddLeft(GameObject arm)
  {
    if (left != null) throw new System.Exception("Could not add arm to left slot. Something already there");
    var armBehaviour = arm.GetComponentOrThrow<ArmBehaviour>();
    if (armBehaviour.owner != ArmBehaviour.ArmOwnerType.User) throw new System.Exception("Could not add arm to left slot. Behaviour must be user");
    var handType = arm.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandLeft) throw new System.Exception("Can not add right arm to left slot");
    var pivot = arm.GetComponentOrThrow<PivotPoint>();

    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    left = arm;
  }

  public void AddRight(GameObject arm)
  {
    if (right != null) throw new System.Exception("Could not add arm to left slot. Something already there");
    var armBehaviour = arm.GetComponentOrThrow<ArmBehaviour>();
    if (armBehaviour.owner != ArmBehaviour.ArmOwnerType.User) throw new System.Exception("Could not add arm to left slot. Behaviour must be user");
    var handType = arm.GetComponentOrThrow<Handedness>().handType;
    if (handType != Handedness.HandTypes.HandRight) throw new System.Exception("Can not add left arm to right slot");
    var pivot = arm.GetComponentOrThrow<PivotPoint>();

    pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
    arm.transform.parent = transform;
    arm.transform.localPosition = Vector3.zero;
    right = arm;
  }
}
