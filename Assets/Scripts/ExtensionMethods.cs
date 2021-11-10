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

  public static void LerpLocal(this Transform transform, Transform target, float strength)
  {
    transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, target.localPosition, strength);
    transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, target.localRotation, strength);
    transform.localScale = Vector3.LerpUnclamped(transform.localScale, target.localScale, strength);
  }
  public static void LerpLocal(this Transform transform, ITransform target, float strength)
  {
    transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, target.localPosition, strength);
    transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, target.localRotation, strength);
    transform.localScale = Vector3.LerpUnclamped(transform.localScale, target.localScale, strength);
  }

  public static void TraverseChildren(this Transform parent, Action<Transform> action)
  {
    foreach (Transform child in parent)
    {
      action(child);
      child.TraverseChildren(action);
    }
  }

  public static List<Transform> AllChildren(this Transform parent)
  {
    var result = new List<Transform>();
    void AddToResult(Transform transform)
    {
      result.Add(transform);
    }

    parent.TraverseChildren(AddToResult);

    return result;
  }

  public static T GetComponentOrThrow<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.TryGetComponent(out T component)) return component;

    throw new Exception($"Component {typeof(T)} not found");
  }

  public static Option<T> GetOptionComponent<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.TryGetComponent(out T component)) return new Some<T>(component);
    return new None<T>();
  }

  public static bool HasComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() != null;
  public static bool HasComponent(this GameObject gameObject, Type componentType) => gameObject.GetComponent(componentType) != null;

  public static T AddIfNotExisting<T>(this GameObject gameObject) where T : Component
  {
    if (gameObject.HasComponent<T>()) return gameObject.GetComponent<T>();
    else return gameObject.AddComponent<T>();
  }
  public static Component AddIfNotExisting(this GameObject gameObject, Type componentType)
  {
    if (gameObject.HasComponent(componentType)) return gameObject.GetComponent(componentType);
    else return gameObject.AddComponent(componentType);
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

  public static Option<TValue> GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
  {
    if (key == null)
    {
      return new None<TValue>();
    }

    if (dictionary.TryGetValue(key, out var result))
    {
      return Option<TValue>.of(result);
    }

    return new None<TValue>();
  }

  public static float Rescale(this float value, float currentMin, float currentMax, float nextMin, float nextMax)
  {
    var currentRange = currentMax - currentMin;
    var currentDistance = (value - currentMin) / currentRange;

    var nextRange = nextMax - nextMin;
    return currentDistance * nextRange + nextMin;
  }

  public static TEnum ConvertToEnum<TEnum>(this string value) where TEnum : struct
  {
    return (TEnum)Enum.Parse(typeof(TEnum), value);
  }

}

public interface ITransform
{
  Vector3 localPosition { get; set; }
  Quaternion localRotation { get; set; }
  Vector3 localScale { get; set; }
}