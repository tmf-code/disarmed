using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MovementToRecordingDictionary : SerializableDictionary<RecordingsStore.RecordedMovements, ArmRecording> { }

public class RecordingsStore : MonoBehaviour
{
  [ShowOnly] public readonly string recordingsPath = "Recordings";
  // Start is called before the first frame update
  public enum RecordedMovements
  {
    Unset,
    LeftAction,
    RightAction
  }

  [SerializeField]
  [HideInInspector]
  private MovementToRecordingDictionary recordings = new MovementToRecordingDictionary();

  void Awake()
  {
    var loadedRecordings = Enum.GetNames(typeof(RecordedMovements)).Select((name, key) =>
      {
        return new { key = (RecordedMovements)key, ArmRecording = LoadRecording((RecordedMovements)key) };
      }).ToDictionary(keyValue => keyValue.key, keyValue => keyValue.ArmRecording);

    recordings.Clear();
    recordings.CopyFrom(loadedRecordings);
  }


  private ArmRecording LoadRecording(RecordedMovements recordingName)
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

  public ArmRecording RandomRecording()
  {
    var values = Enum.GetValues(typeof(RecordedMovements));
    var random = UnityEngine.Random.value;
    RecordedMovements randomRecording = (RecordedMovements)values.GetValue((int)Mathf.Floor(values.Length * random));
    return recordings[randomRecording];
  }
}
