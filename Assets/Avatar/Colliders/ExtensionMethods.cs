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

  public static bool HasComponent<T>(this GameObject gameObject) where T : Component
  {
    return gameObject.GetComponent<T>() != null;
  }

  public static T AddIfNotExisting<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.HasComponent<T>())
    {
      return gameObject.GetComponent<T>();
    }
    else
    {
      return gameObject.AddComponent<T>();
    }
  }

  public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
  {
    var maybeComponent = gameObject.GetComponent<T>();
    GameObject.Destroy(maybeComponent);
  }

}
