using System.Collections.Generic;
using UnityEngine;

public class PlinthArms : MonoBehaviour
{
  public ArmBehaviour[] all = new ArmBehaviour[30];
  public ArmBehaviour[] eleven = new ArmBehaviour[11];

  public enum PlinthArmCount
  {
    None,
    All,
    Eleven,
  }

  public void SetArmCount(PlinthArmCount count)
  {
    switch (count)
    {
      case PlinthArmCount.None:
        foreach (var arm in all)
        {
          arm.gameObject.SetActive(false);
        }
        break;
      case PlinthArmCount.All:
        foreach (var arm in all)
        {
          arm.gameObject.SetActive(true);
        }
        break;
      case PlinthArmCount.Eleven:
        HashSet<ArmBehaviour> included = new HashSet<ArmBehaviour>();
        HashSet<ArmBehaviour> excluded = new HashSet<ArmBehaviour>(all);
        foreach (var arm in eleven)
        {
          excluded.Remove(arm);
          included.Add(arm);
        }

        foreach (var item in included)
        {
          item.gameObject.SetActive(true);
        }

        foreach (var item in excluded)
        {
          item.gameObject.SetActive(false);
        }
        break;
    }
  }


}
