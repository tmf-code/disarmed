using System;
using UnityEngine;

public struct CollisionPair : IEquatable<CollisionPair>
{
  private readonly GameObject left;
  readonly GameObject right;

  public CollisionPair(GameObject left, GameObject right)
  {
    this.left = left;
    this.right = right;
  }

  public bool Equals(CollisionPair collision)
  {
    return left == collision.left && right == collision.right;
  }
}
