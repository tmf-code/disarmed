using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MovementToRecordingDictionary : SerializableDictionary<RecordingsStore.RecordedMovements, ObjectToFramesDictionary> { }

[Serializable]
public class ObjectToFramesDictionary : SerializableDictionary<string, UnSerializedTransform[]> { }

public class RecordingsStore : MonoBehaviour
{
  [ShowOnly] public readonly string recordingsPath = "Compressed";

  public Timeline timeline;
  // Start is called before the first frame update
  public enum RecordedMovements
  {
    // Act 2 start
    wavingLeft1,
    wavingLeft2,
    wavingLeft3,
    wavingLeft4,
    wavingLeft5,
    wavingLeft6,
    wavingRight1,
    wavingRight2,
    wavingRight3,
    wavingRight4,
    wavingRight5,

    // Act 2 end
    // directions,
    // directions2,
    // directions3,
    // discussing,
    // discussing2,
    // discussing3,
    // discussing4,
    // discussing5,
    // eating,
    // greeting,
    // greeting2,
    // photo,
    // photo2,
    // posing,
    // posing2,
    // posing3,
    // purchases,
    // smoking,
    // smoking2,
    // waiting,
    // waiting2,
    // walking,
    // walking2,

    // Act 4
    // sub1,
    // sub2,
    // sub3,
    // sub4,
    // sub5,
    // sub6,
    // sub7,
    // sub8,
    // sub9,
    // sub10,
    // sub11,
    // sub12,
    // sub13,
    // sub14,
    // sub15,
    // sub16,
    // sub17,
    // sub18,
    // sub19,
  }
  [SerializeField]
  [HideInInspector]
  private MovementToRecordingDictionary leftRecordings;
  [SerializeField]
  [HideInInspector]
  private MovementToRecordingDictionary rightRecordings;

  void Awake()
  {
    leftRecordings = new MovementToRecordingDictionary();
    rightRecordings = new MovementToRecordingDictionary();
    var keyValue = EnumToKeyValue<RecordedMovements>().ToList();
    keyValue.ForEach((keyValue) =>
      {
        var key = keyValue.Key;
        var name = keyValue.Value;
        var armRecording = LoadRecording(name);
        var reorganizedRecording = OrganiseRecordingByKey(armRecording);

        if (armRecording.hand == Handedness.HandTypes.HandLeft)
        {
          leftRecordings.Add(key, reorganizedRecording);
        }
        else
        {
          rightRecordings.Add(key, reorganizedRecording);
        }
      });

    Debug.Log(leftRecordings.Count);
  }

  private CompressedArmRecording LoadRecording(string recordingName)
  {
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
    var recordingPath = $"{recordingsPath}/{recordingName}";
    var textAsset = Resources.Load<TextAsset>(recordingPath);
    if (textAsset == null)
    {
      throw new Exception($"Could not load text asset {recordingPath}");
    }

    var maybeArmRecording = JsonUtility.FromJson<CompressedArmRecording>(textAsset.text);
    if (maybeArmRecording == null)
      throw new Exception($"Could not parse text asset {textAsset} with name {recordingName} \n {textAsset.text}");
    return maybeArmRecording;
  }

  private ObjectToFramesDictionary OrganiseRecordingByKey(CompressedArmRecording recording)
  {
    var result = new ObjectToFramesDictionary();
    for (var frameIndex = 0; frameIndex < recording.frameTransforms.Count; frameIndex++)
    {
      var frame = recording.frameTransforms[frameIndex];
      foreach (var nameTransform in frame)
      {
        if (result.TryGetValue(nameTransform.Key, out var frames))
        {
          frames[frameIndex] = nameTransform.Value;
        }
        else
        {
          var transforms = new UnSerializedTransform[recording.frameTransforms.Count];
          transforms[frameIndex] = nameTransform.Value;
          result.Add(nameTransform.Key, transforms);

        }
      }
    }

    return result;
  }

  public ObjectToFramesDictionary RandomRecording(Handedness.HandTypes handType)
  {

    List<RecordedMovements> L(params RecordedMovements[] array) => array.ToList();
    if (handType == Handedness.HandTypes.HandLeft)
    {
      var values = leftRecordings.Values;
      if (values.Count == 0)
      {
        throw new Exception("No recordings for left hand");
      }
      var random = UnityEngine.Random.value;
      return values.ElementAt((int)Mathf.Floor(values.Count * random));
    }
    else
    {
      var values = rightRecordings.Values;

      if (values.Count == 0)
      {
        throw new Exception("No recordings for right hand");
      }

      var random = UnityEngine.Random.value;
      return values.ElementAt((int)Mathf.Floor(values.Count * random));
    }
  }

  static KeyValuePair<TEnum, string>[] EnumToKeyValue<TEnum>() where TEnum : struct
  {
    var names = Enum.GetNames(typeof(TEnum));
    var keyValues = names.Select(name =>
    {
      if (Enum.TryParse(name, false, out TEnum result))
      {
        return new KeyValuePair<TEnum, string>(result, name);
      }
      else
      {
        throw new InvalidOperationException();
      }
    }).ToArray();

    return keyValues;
  }
}
