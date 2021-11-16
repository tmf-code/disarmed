using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShoulderReplacementRecorder : MonoBehaviour
{
  public GameObject arm;
  [Range(1, 60)]
  public int recordingFrameRate = 1;
  public string recordingName = "unset";
  public bool startRecording = false;
  [ShowOnly] public bool isRecording = false;
  public bool stopRecording = false;
  public int framesSaved = 0;
  private ArmRecording recording;
  public Transform shoulderTarget;

  void AppendFrame()
  {
    framesSaved += 1;

    var model = arm.transform.FindRecursiveOrThrow("Model");
    var transforms = new List<SerializedTransform>();

    void Serialize(Transform transform)
    {
      transforms.Add(new SerializedTransform(transform));
    }

    var rotation = Quaternion.Inverse(shoulderTarget.rotation) * model.rotation;
    var data = new List<double>(10) {
      model.localPosition.x,
      model.localPosition.y,
      model.localPosition.z,
      rotation.x,
      rotation.y,
      rotation.z,
      rotation.w,
      model.localScale.x,
      model.localScale.y,
      model.localScale.z,
     };
    transforms.Add(new SerializedTransform(data, model.name));

    // Serialize(start);
    model.TraverseChildren(Serialize);
    var serializedTransforms = new SerializedTransforms(transforms);
    recording.serializedFrameTransforms.Add(serializedTransforms);
  }

  void SaveRecording()
  {
    var json = JsonUtility.ToJson(recording);
    var filepath = $"{Application.dataPath}/Resources/Compressed/{recordingName}.json";
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
      recording = new ArmRecording(recordingFrameRate, arm.GetComponentOrThrow<Handedness>().handType);
      framesSaved = 0;
      InvokeRepeating(nameof(AppendFrame), 0f, 1F / recordingFrameRate);
      startRecording = false;
      isRecording = true;
    }

    startRecording = false;
    stopRecording = false;
  }
}
