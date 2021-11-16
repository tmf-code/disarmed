using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyArmDropMovement : MonoBehaviour
{
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;
  [ShowOnly] private int playbackFrameRate = 10;
  [ShowOnly] private int framesPlayed = 0;
  [ShowOnly] private float strength = 10F / 60F;

  private ObjectToFramesDictionary recording;

  public ChildDictionary childDictionary;
  private Dictionary<GameObject, UnSerializedTransform[]> recordingPairs;

  void Start()
  {
    recording = GameObject.Find("Recordings")
      .GetComponentOrThrow<RecordingsStore>().act4ArmDropRight
      .UnwrapOrLoad()
      .GetValue(Act4ArmDropRight.armDrop2Right)
      .Unwrap();

    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    handBonePairs = childDictionary.handBonePairs;
    LoadAndPlay();
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

  void Update()
  {
    // Apply the recording to the VR tracking object
    foreach (var pair in recordingPairs)
    {
      var gameObject = pair.Key;
      var transform = pair.Value;

      if (gameObject.name == "Model")
      {
        gameObject.transform.LerpLocal(transform[framesPlayed], strength);
      }
      else
      {
        gameObject.transform.localRotation = Quaternion.SlerpUnclamped(gameObject.transform.localRotation, transform[framesPlayed].localRotation, strength);
      }
    }
  }
}
