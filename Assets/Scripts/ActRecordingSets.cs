using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ActRecordingSets
{
  None,
  Act1Movements,
  Act2StartMovementsLeft,
  Act2StartMovementsRight,
  Act2EndMovementsLeft,
  Act2EndMovementsRight,
  Act3Movements,
  Act4,
}

public enum Act1Movements
{
  upperHandRight,
}

public enum Act2StartMovementsLeft
{
  wavingLeft2,
  wavingLeft3,
  wavingLeft4,
  wavingLeft5,
  wavingLeft6,
}
public enum Act2StartMovementsRight
{
  wavingRight1,
  wavingRight2,
  wavingRight3,
  wavingRight4,
  wavingRight5,
}

public enum Act2EndMovementsLeft
{
  posing3Left,
  purchasesLeft,
  smoking2Left,
}
public enum Act2EndMovementsRight
{
  directions3Right,
  discussing5Right,
  eatingRight,
  greeting2Right,
  photo2Right,
}

public enum Act3Movements
{
  replacementShoulderLeft,
  replacementShoulderRight,
}

public enum Act4ArmDropRight
{
  armDrop1Right,
  armDrop2Right,
}

public enum Act4MovementsLeft
{
  sub11Left,
  sub12Left,
  sub13Left,
  sub14Left,
  sub15Left,
  sub16Left,
}

public enum Act4MovementsRight
{
  sub1Right,
  sub2Right,
  sub3Right,
  sub4Right,
  sub5Right,
  sub6Right,
}

public class ActMovements
{
  public static CompressedArmRecording LoadRecording(string recordingName)
  {
    var recordingPath = $"Recordings/{recordingName}";
    var textAsset = Resources.Load<TextAsset>(recordingPath);
    if (textAsset == null)
    {
      throw new Exception($"Could not load text asset {recordingPath}");
    }

    var maybeArmRecording = JsonUtility.FromJson<CompressedArmRecording>(textAsset.text);
    if (maybeArmRecording.frameTransforms.Count == 0)
      throw new Exception($"Could not parse text asset {textAsset} with name {recordingName} \n {textAsset.text}");

    Resources.UnloadAsset(textAsset);
    return maybeArmRecording;
  }

  public static Dictionary<TEnum, ObjectToFramesDictionary> LoadRecordings<TEnum>() where TEnum : struct
  {
    return Enum.GetNames(typeof(TEnum))
      .Select(name => new { name, recording = LoadRecording(name) }).Select(nameRecording =>
      {
        var name = nameRecording.name;
        var recording = nameRecording.recording;
        var dict = OrganiseRecordingByKey(recording);

        if (!Enum.TryParse<TEnum>(name, true, out var enumKey))
        {
          throw new Exception($"Could not parse name {name} into enum {typeof(TEnum)}");
        }
        return new KeyValuePair<TEnum, ObjectToFramesDictionary>(enumKey, dict);
      })
      .ToDictionary(kv => kv.Key, kv => kv.Value);
  }

  public static ObjectToFramesDictionary OrganiseRecordingByKey(CompressedArmRecording recording)
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

}


public struct MovementNames
{
  public static List<string> names = Enum.GetNames(typeof(Act1Movements))
    .Concat(Enum.GetNames(typeof(Act2StartMovementsLeft)))
    .Concat(Enum.GetNames(typeof(Act2StartMovementsRight)))
    .Concat(Enum.GetNames(typeof(Act2EndMovementsLeft)))
    .Concat(Enum.GetNames(typeof(Act2EndMovementsRight)))
    .Concat(Enum.GetNames(typeof(Act3Movements)))
    .Concat(Enum.GetNames(typeof(Act4ArmDropRight)))
    .Concat(Enum.GetNames(typeof(Act4MovementsLeft)))
    .Concat(Enum.GetNames(typeof(Act4MovementsRight))).ToList();
}
