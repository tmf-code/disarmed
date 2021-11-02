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
    }

    if (startPlaying == true && !isPlaying)
    {
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
    var start = arm.transform.FindRecursiveOrThrow("Model");
    void ApplyTransform(Transform current)
    {
      var filtered = unSerializedTransforms.Where(target => target.name == current.name);


      var doesBoneExist = filtered.Count() != 0;
      if (!doesBoneExist)
      {
        return;
      }

      var target = filtered.ElementAt(0);

      current.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, strength);
      current.localRotation = Quaternion.Slerp(current.localRotation, target.localRotation, strength);
      current.localScale = Vector3.Lerp(current.localScale, target.localScale, strength);

    }
    // Apply root transform
    // ApplyTransform(start);
    start.TraverseChildren(ApplyTransform);
  }
}
