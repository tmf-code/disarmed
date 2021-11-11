using System;
using UnityEngine;

[Serializable]
public struct TransformPair
{
  public Transform Item1;
  public Transform Item2;

  public TransformPair(Transform item1, Transform item2)
  {
    Item1 = item1;
    Item2 = item2;
  }
}
