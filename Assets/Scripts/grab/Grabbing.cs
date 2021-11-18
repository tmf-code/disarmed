using System;
using UnityEngine;

public class Grabbing : MonoBehaviour, PairedAction
{
  public Grabbed other;
  public float minimumTime = 2;
  public float duration = 0;

  PairedAction PairedAction.other { get => other; }
  public Func<bool> isHandOpen;

  void Update()
  {
    duration += Time.deltaTime;
    if (isHandOpen() && duration > minimumTime)
    {
      OnReleaseBegin();
    }
  }

  public void OnReleaseBegin()
  {
    GrabOperations.GrabToReleaseIntent(this, other);
  }

  public void OnAttachBegin(Func<bool> isColliding)
  {
    if (duration < minimumTime) return;
    GrabOperations.GrabToAttachIntent(this, other, isColliding);
  }
}

