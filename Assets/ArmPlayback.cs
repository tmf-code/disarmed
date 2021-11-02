using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ArmPlayback : MonoBehaviour
{
  public GameObject arm;
  [Range(0F, 1F)]
  public float strength = 1;
  [Range(1, 60)]
  public int playbackFrameRate = 1;

  [ShowOnly] public string recordingsPath = "Recordings";

  public string recordingName = "unset";
  public bool startPlaying = false;
  [ShowOnly] public bool isPlaying = false;
  public bool stopPlaying = false;
  public int framesPlayed = 0;

  private ArmRecording recording;
  private Transform model;

  public void Start()
  {
    LoadRecording();
  }


  public void LoadRecording()
  {
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
    var recordingPath = $"{recordingsPath}/{recordingName}";
    var textAsset = Resources.Load<TextAsset>(recordingPath);
    if (textAsset == null)
    {
      throw new System.Exception($"Could not load text asset {recordingPath}");
    }
    recording = JsonUtility.FromJson<ArmRecording>(textAsset.text);
  }


  void PlayNextFrame()
  {

    var isFrameAvaliable = framesPlayed >= recording.frameTransforms.Count;
    if (isFrameAvaliable)
    {
      stopPlaying = true;
      return;
    }

    framesPlayed += 1;
  }

  // Update is called once per frame
  void Update()
  {

    if (stopPlaying)
    {
      CancelInvoke();
      stopPlaying = false;
      isPlaying = false;
      return;
    }

    if (startPlaying == true && !isPlaying)
    {
      Debug.Log("Start repeating invoke");
      LoadRecording();
      framesPlayed = 0;
      InvokeRepeating(nameof(PlayNextFrame), 0f, 1F / playbackFrameRate);
      startPlaying = false;
      isPlaying = true;
    }


    startPlaying = false;
    stopPlaying = false;

    if (!isPlaying) return;

    var isFrameAvaliable = framesPlayed >= recording.frameTransforms.Count;
    if (isFrameAvaliable)
    {
      stopPlaying = true;
      return;
    }

    ApplyTransforms();
  }

  private void ApplyTransforms()
  {
    var unSerializedTransforms = recording.frameTransforms[framesPlayed];
    model = model == null ? arm.transform.FindRecursiveOrThrow("Model") : model;
    ApplyTransform(model, true, unSerializedTransforms);
    model.TraverseChildren(current => ApplyTransform(current, false, unSerializedTransforms));
  }

  private void ApplyTransform(Transform current, bool rotationOnly, Dictionary<string, UnSerializedTransform> unSerializedTransforms)
  {
    if (unSerializedTransforms.TryGetValue(current.name, out var target))
    {
      if (!rotationOnly)
      {
        current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
        current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);
      }
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
    }

  }
}
