using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AudioSourceExtensions
{
  public static void FadeOut(this AudioSource a, float duration)
  {
    a.GetComponent<MonoBehaviour>().StartCoroutine(FadeOutCore(a, duration));
  }
  public static void FadeIn(this AudioSource a, float duration)
  {
    a.GetComponent<MonoBehaviour>().StartCoroutine(FadeInCore(a, duration));
  }

  private static IEnumerator FadeOutCore(AudioSource a, float duration)
  {
    while (a.volume > 0)
    {
      a.volume -= Time.deltaTime / duration;
      yield return new WaitForEndOfFrame();
    }
  }

  private static IEnumerator FadeInCore(AudioSource a, float duration)
  {
    while (a.volume < 1)
    {
      a.volume += Time.deltaTime / duration;
      yield return new WaitForEndOfFrame();
    }

  }
}

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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void LerpLocal(this Transform transform, Transform target, float strength)
  {
    transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, target.localPosition, strength);
    transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, target.localRotation, strength);
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void LerpLocal(this Transform transform, ITransform target, float strength)
  {
    transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, target.localPosition, strength);
    transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, target.localRotation, strength);
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

  public static bool HasComponent<T>(this Component component) where T : Component => component.GetComponent<T>() != null;
  public static bool HasComponent(this Component component, Type componentType) => component.GetComponent(componentType) != null;

  public static T AddIfNotExisting<T>(this Component component) where T : Component
  {
    if (component.HasComponent<T>()) return component.GetComponent<T>();
    else return component.gameObject.AddComponent<T>();
  }
  public static Component AddIfNotExisting(this Component component, Type componentType)
  {
    if (component.HasComponent(componentType)) return component.GetComponent(componentType);
    else return component.gameObject.AddComponent(componentType);
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

  public static Option<T> RemoveComponent<T>(this GameObject gameObject) where T : Component
  {
    return Option<T>.of(gameObject.GetComponent<T>()).Chain(UnityEngine.Object.Destroy);
  }

  public static Option<T> RemoveComponent<T>(this T component) where T : Component
  {
    UnityEngine.Object.Destroy(component);
    return Option<T>.of(component);
  }

  public static Option<T> Remove<T>(this T component) where T : Component
  {
    GameObject.Destroy(component);
    return Option<T>.of(component);
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

  public static T PopAt<T>(this List<T> list, int index)
  {
    T r = list[index];
    list.RemoveAt(index);
    return r;
  }

  public static T RandomElement<T>(this T[] list)
  {
    if (list.Length == 0) throw new IndexOutOfRangeException("Could not get random element from empty array");
    var index = Random.Range(0, list.Length);
    return list[index];
  }
  public static TValue RandomElement<TKey, TValue>(this IDictionary<TKey, TValue> dict)
  {
    var index = Random.Range(0, dict.Count);
    List<TValue> values = Enumerable.ToList(dict.Values);
    return values.ElementAt(index);
  }
}

public interface ITransform
{
  Vector3 localPosition { get; set; }
  Quaternion localRotation { get; set; }
}