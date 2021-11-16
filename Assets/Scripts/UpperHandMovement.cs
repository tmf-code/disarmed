using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UpperHandMovement : MonoBehaviour
{
  public ChildDictionary childDictionary;
  [SerializeField]
  [HideInInspector]
  protected TransformPair[] handBonePairs;
  [ShowOnly] protected int playbackFrameRate = 10;
  [ShowOnly] protected int framesPlayed = 0;
  [ShowOnly] protected float strength = 10F / 60F;

  private ObjectToFramesDictionary recording;
  private Dictionary<GameObject, UnSerializedTransform[]> recordingPairs;

  private static Dictionary<GameObject, UnSerializedTransform[]> RecordingToPairedTarget(ObjectToFramesDictionary recording, Transform root, StringGameObjectDictionary targets)
  {
    Dictionary<GameObject, UnSerializedTransform[]> result = new Dictionary<GameObject, UnSerializedTransform[]>();

    foreach (var nameTransforms in recording)
    {
      var name = nameTransforms.Key;
      var transforms = nameTransforms.Value;
      if (name.Contains("handMeshNode"))
      {
        continue;
      }
      var target = name == "Model" ? root.gameObject : targets.GetValue(name).Unwrap();
      result.Add(target, transforms);
      continue;
    }

    return result;
  }

  void LoadAndPlay()
  {

    recordingPairs = RecordingToPairedTarget(
        recording,
        childDictionary.model.transform,
        childDictionary.modelChildren
    );

    CancelInvoke();
    InvokeRepeating(nameof(PlayNextFrame), 0f, 1F / playbackFrameRate);
  }

  void PlayNextFrame()
  {
    try
    {
      var numFrames = recording.First().Value.Length;
      framesPlayed += 1;
      framesPlayed %= numFrames;

    }
    catch
    {
      throw new System.NullReferenceException();
    }
  }

  void Start()
  {
    recording = GetRecording();
    handBonePairs = childDictionary.handBonePairs;
    LoadAndPlay();
  }

  public abstract ObjectToFramesDictionary GetRecording();

  void Update()
  {
    // Apply the recording to the VR tracking object
    foreach (var pair in recordingPairs)
    {
      var current = pair.Key;
      var position = current.transform.localPosition;
      var rotation = current.transform.localRotation;
      var transforms = pair.Value;
      var target = transforms[framesPlayed];
      if (current.name != "Model")
      {
        current.transform.localPosition = Vector3.LerpUnclamped(position, target.localPosition, strength);
      }
      current.transform.localRotation = Quaternion.SlerpUnclamped(rotation, target.localRotation, strength);
    }
  }
}
