using System.Collections.Generic;
using UnityEngine;

public class EmitterPool : MonoBehaviour
{
  public int maxSize = 5;
  private List<AudioSource> available;
  private List<AudioSource> occupied;

  public GameObject emitterPrefab;

  public void Start()
  {
    available = new List<AudioSource>(maxSize);
    occupied = new List<AudioSource>(0);

    for (int i = 0; i < maxSize; i++)
    {
      var gameObject = Instantiate(emitterPrefab, transform);
      var source = gameObject.GetComponentOrThrow<AudioSource>();
      available.Add(source);
    }
  }

  public void TryPlayAtPosition(AudioClip clip, Vector3 position)
  {
    TryPlayAtPosition(clip, position, 1F);
  }

  public void TryPlayAtPosition(AudioClip clip, Vector3 position, float volume)
  {
    if (available.Count == 0) return;
    var source = available.PopAt(0);
    occupied.Add(source);
    source.transform.position = position;
    source.PlayOneShot(clip, volume);
  }

  public void Update()
  {
    for (var i = 0; i < occupied.Count; i++)
    {
      var source = occupied[i];

      if (!source.isPlaying)
      {
        occupied.RemoveAt(i);
        available.Add(source);
      }
    }
  }
}
