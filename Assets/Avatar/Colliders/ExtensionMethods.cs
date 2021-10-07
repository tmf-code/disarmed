using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
  public static Transform FindRecursiveOrThrow(this Transform transform, string childName)
  {
    var maybeChild = transform.FindChildRecursive(childName);

    if (maybeChild == null) throw new Exception($"Could not find child: {childName}");

    return maybeChild;
  }

  public static Transform FindOrThrow(this Transform transform, string childName)
  {
    var maybeChild = transform.Find(childName);

    if (maybeChild == null) throw new Exception($"Could not find child: {childName}");

    return maybeChild;
  }

  public static void TraverseChildren(this Transform parent, Action<Transform> action)
  {
    foreach (Transform child in parent)
    {
      action(child);
      child.TraverseChildren(action);
    }
  }

  public static T GetComponentOrThrow<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.TryGetComponent(out T component)) return component;

    throw new Exception($"Component {component.GetType()} not found");
  }

  public static bool HasComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() != null;

  public static T AddIfNotExisting<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.HasComponent<T>()) return gameObject.GetComponent<T>();
    else return gameObject.AddComponent<T>();
  }

  public static T GetComponentIfNull<T>(this GameObject gameObject, T component) where T : Component
  {
    if (component != null) return component;
    return gameObject.GetComponent<T>();
  }

  public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
  {
    var maybeComponent = gameObject.GetComponent<T>();
    GameObject.Destroy(maybeComponent);
  }

  public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
  {
    dictionary.TryGetValue(key, out var result);
    return result;
  }

  public static float Rescale(this float value, float currentMin, float currentMax, float nextMin, float nextMax)
  {
    var currentRange = currentMax - currentMin;
    var currentDistance = (value - currentMin) / currentRange;

    var nextRange = nextMax - nextMin;
    return currentDistance * nextRange + nextMin;
  }
}
