using System;
using UnityEngine;

public static class ExtensionMethods
{
  public static Transform FindRecursiveOrThrow(this Transform transform, string childName)
  {
    var maybeChild = transform.FindChildRecursive(childName);

    if (maybeChild == null)
    {
      throw new Exception($"Could not find child: {childName}");
    }

    return maybeChild;
  }
}
