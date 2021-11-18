using System;
using UnityEngine;

public class AwaitingGrabbing : MonoBehaviour
{
  public float minimumTime = 1;
  public float duration = 0;

  void Update()
  {
    duration = Mathf.Clamp(duration + Time.deltaTime, 0, 1);
  }

  public void OnGrabBegin(AwaitingGrabbed awaitingGrabbed, Func<bool> isColliding)
  {
    if (duration < minimumTime) return;
    GrabOperations.AwaitingToGrabIntent(this, awaitingGrabbed, isColliding);
  }
}


