using System.Collections.Generic;
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

  public bool playOnEnable = false;
  public int framesPlayed = 0;

  public int startAtFrame = 0;
  public int endAtFrame = 0;

  public PivotPoint pivotPoint;

  private ArmRecording recording;
  private Transform model;

#if UNITY_EDITOR
  [Button(nameof(ApplyFrame))]
  public bool buttonField;
#endif


  public Animator animator;

  public void Start()
  {
    LoadRecording();
  }

#if UNITY_EDITOR
  public void ApplyFrame()
  {
    LoadRecording();
    if (endAtFrame > recording.frameTransforms.Count)
      throw new System.Exception($"End at frame cannot be higher than total frames {endAtFrame}:{recording.frameTransforms.Count}");

    framesPlayed %= endAtFrame;
    if (framesPlayed == 0)
    {
      framesPlayed += startAtFrame;
    }
    ApplyTransforms();
    pivotPoint.LateUpdate();
  }
#endif

  void OnDisable()
  {
    CancelInvoke();
  }

  void OnEnable()
  {
    if (playOnEnable)
    {
      startPlaying = true;
    }
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
    if (framesPlayed == 0)
    {
      framesPlayed += startAtFrame;
      animator.Play("LowerArm", 0);
    }


    var isFrameAvaliable = framesPlayed >= recording.frameTransforms.Count;
    if (isFrameAvaliable)
    {
      stopPlaying = true;
      return;
    }

    framesPlayed += 1;
    framesPlayed %= endAtFrame;
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
        current.localPosition = Vector3.LerpUnclamped(current.localPosition, target.localPosition, strength);
      }
      current.localRotation = Quaternion.SlerpUnclamped(current.localRotation, target.localRotation, strength);
    }

  }
}
