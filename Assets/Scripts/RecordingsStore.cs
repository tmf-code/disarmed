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
  // [ShowOnly] public readonly string recordingsPath = "Recordings/Act2-actions";
  [ShowOnly] public readonly string recordingsPath = "Recordings/Act4";
  // Start is called before the first frame update
  public enum RecordedMovements
  {
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

    sub1,
    sub2,
    sub3,
    sub4,
    sub5,
    sub6,
    sub7,
    sub8,
    sub9,
    sub10,
    sub11,
    sub12,
    sub13,
    sub14,
    sub15,
    sub16,
    sub17,
    sub18,
    sub19,
    sub20,
  }

  [SerializeField]
  private MovementToRecordingDictionary leftRecordings;
  [SerializeField]
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

  private ArmRecording LoadRecording(string recordingName)
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
    return JsonUtility.FromJson<ArmRecording>(textAsset.text);
  }

  private ObjectToFramesDictionary OrganiseRecordingByKey(ArmRecording recording)
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
