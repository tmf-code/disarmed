using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ArmRecording : ISerializationCallbackReceiver
{
  public List<SerializedTransforms> serializedFrameTransforms = new List<SerializedTransforms>();
  public int recordingFrameRate;
  public Handedness.HandTypes hand;

  public List<Dictionary<string, UnSerializedTransform>> frameTransforms;
  public ArmRecording(int recordingFrameRate, Handedness.HandTypes hand)
  {
    serializedFrameTransforms = new List<SerializedTransforms>();
    this.recordingFrameRate = recordingFrameRate;
    this.hand = hand;
  }

  public void OnAfterDeserialize()
  {
    frameTransforms = serializedFrameTransforms
      .ConvertAll(data => data.transforms
        .ConvertAll(transform => transform.unSerialized)
        .GroupBy(value => value.name)
        .ToDictionary(value => value.Key, value => value.First())
        );
  }

  public void OnBeforeSerialize()
  {
  }

}

