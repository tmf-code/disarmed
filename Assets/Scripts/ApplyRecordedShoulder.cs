using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyRecordedShoulder : MonoBehaviour
{
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;
  [ShowOnly] private int playbackFrameRate = 10;
  [ShowOnly] private int framesPlayed = 0;
  [ShowOnly] private float strength = 10F / 60F;

  private ObjectToFramesDictionary recording;

  private ChildDictionary childDictionary;
  private DataSources dataSources;
  private Dictionary<GameObject, UnSerializedTransform[]> recordingPairs;

  void Start()
  {
    var hand = gameObject.GetComponentOrThrow<Handedness>();
    recording = Option<GameObject>.of(GameObject.Find("Recordings"))
      .Unwrap()
      .GetOptionComponent<RecordingsStore>()
      .Map((recording) => recording.act3Movements.UnwrapOrLoad().GetValue(hand.IsLeft()
        ? Act3Movements.replacementShoulderLeft
        : Act3Movements.replacementShoulderRight))
      .Unwrap()
      .Unwrap();

    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    dataSources = gameObject.GetComponentOrThrow<DataSources>();
    handBonePairs = childDictionary.handBonePairs;

    gameObject.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Shoulder;
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

  void OnDestroy()
  {
    gameObject.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Wrist;
    transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    CancelInvoke();
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
      var objectToSet = pair.Key;
      var transformsPerFrame = pair.Value;

      if (objectToSet.name == "Model")
      {
        var position = Vector3.LerpUnclamped(
          objectToSet.transform.localPosition,
          transformsPerFrame[framesPlayed].localPosition,
          strength);

        var rotation = Quaternion.SlerpUnclamped(
          objectToSet.transform.localRotation,
          dataSources.ikArmBoneData.target.rotation * transformsPerFrame[framesPlayed].localRotation,
          strength);

        objectToSet.transform.localPosition = position;
        objectToSet.transform.localRotation = rotation;
      }
      else
      {
        objectToSet.transform.localRotation = Quaternion.SlerpUnclamped(
          objectToSet.transform.localRotation,
          transformsPerFrame[framesPlayed].localRotation,
          strength);
      }
    }

    transform.position = dataSources.ikArmBoneData.target.position;
  }
}
