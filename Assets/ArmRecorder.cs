using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArmRecorder : MonoBehaviour
{
  public GameObject arm;
  [Range(1, 60)]
  public int recordingFrameRate = 1;
  [Range(1, 60)]
  public int recordingLengthSeconds = 1;
  public string recordingName = "unset";
  public bool startRecording = false;
  [ShowOnly] public bool isRecording = false;
  public bool stopRecording = false;
  public int framesSaved = 0;
  public ArmRecording recording;

  void AppendFrame()
  {
    framesSaved += 1;

    var start = arm.transform.FindRecursiveOrThrow("Model");
    var transforms = new List<SerializedTransform>();

    void Serialize(Transform transform)
    {
      transforms.Add(new SerializedTransform(transform));
    }

    Serialize(start);
    start.TraverseChildren(Serialize);
    var serializedTransforms = new SerializedTransforms(transforms);
    recording.serializedFrameTransforms.Add(serializedTransforms);
  }

  void SaveRecording()
  {
    var json = JsonUtility.ToJson(recording);
    var filepath = $"{Application.dataPath}/Resources/Recordings/{recordingName}.json";
    Debug.Log($"Writing to {filepath}");
    System.IO.File.WriteAllText(filepath, json);
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
  }

  // Update is called once per frame
  void Update()
  {
    if (stopRecording)
    {
      SaveRecording();
      CancelInvoke();
      stopRecording = false;
      isRecording = false;
    }
    if (startRecording == true && !isRecording)
    {
      recording = new ArmRecording(recordingFrameRate);
      framesSaved = 0;
      InvokeRepeating(nameof(AppendFrame), 0f, 1F / recordingFrameRate);
      startRecording = false;
      isRecording = true;
    }

    startRecording = false;
    stopRecording = false;
  }
}
