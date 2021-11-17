using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ObjectToFramesDictionary : SerializableDictionary<string, UnSerializedTransform[]> { }

public class RecordingsStore : MonoBehaviour
{
  [ShowOnly] public readonly string recordingsPath = "Recordings";

  public Timeline timeline;

  public readonly Loadable<ObjectToFramesDictionary> upperHandRight = new Loadable<ObjectToFramesDictionary>(() =>
  {
    var recording = ActMovements.LoadRecording(Act1Movements.upperHandRight.ToString());
    return ActMovements.OrganiseRecordingByKey(recording);
  });

  public readonly Loadable<Dictionary<Act2StartMovementsLeft, ObjectToFramesDictionary>> act2StartMovementsLeft
    = new Loadable<Dictionary<Act2StartMovementsLeft, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act2StartMovementsLeft>());

  public readonly Loadable<Dictionary<Act2StartMovementsRight, ObjectToFramesDictionary>> act2StartMovementsRight
    = new Loadable<Dictionary<Act2StartMovementsRight, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act2StartMovementsRight>());

  public readonly Loadable<Dictionary<Act2EndMovementsLeft, ObjectToFramesDictionary>> act2EndMovementsLeft
    = new Loadable<Dictionary<Act2EndMovementsLeft, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act2EndMovementsLeft>());

  public readonly Loadable<Dictionary<Act2EndMovementsRight, ObjectToFramesDictionary>> act2EndMovementsRight
    = new Loadable<Dictionary<Act2EndMovementsRight, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act2EndMovementsRight>());

  public readonly Loadable<Dictionary<Act3Movements, ObjectToFramesDictionary>> act3Movements
    = new Loadable<Dictionary<Act3Movements, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act3Movements>());

  public readonly Loadable<Dictionary<Act4ArmDropRight, ObjectToFramesDictionary>> act4ArmDropRight
    = new Loadable<Dictionary<Act4ArmDropRight, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act4ArmDropRight>());

  public readonly Loadable<Dictionary<Act4MovementsLeft, ObjectToFramesDictionary>> act4MovementsLeft
    = new Loadable<Dictionary<Act4MovementsLeft, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act4MovementsLeft>());

  public readonly Loadable<Dictionary<Act4MovementsRight, ObjectToFramesDictionary>> act4MovementsRight
    = new Loadable<Dictionary<Act4MovementsRight, ObjectToFramesDictionary>>(() => ActMovements.LoadRecordings<Act4MovementsRight>());

  void Awake()
  {
    upperHandRight.Load();
    act2StartMovementsLeft.Load();
    act2StartMovementsRight.Load();
  }

  void Update()
  {
    if (timeline.act > Timeline.Acts.Opening) upperHandRight.Load();
    if (timeline.act >= Timeline.Acts.CloseRoof1)
    {
      act2StartMovementsLeft.Load();
      act2StartMovementsRight.Load();
    }

    if (timeline.act >= Timeline.Acts.InactiveRagdollArmsInCenter)
    {
      act2EndMovementsLeft.Load();
      act2EndMovementsRight.Load();
    }

    if (timeline.act >= Timeline.Acts.Three) act3Movements.Load();
    if (timeline.act >= Timeline.Acts.ThreeEnd) act4ArmDropRight.Load();
    if (timeline.act >= Timeline.Acts.Four)
    {
      act4MovementsLeft.Load();
      act4MovementsRight.Load();
    }
  }

  public ObjectToFramesDictionary RandomRecording(Handedness.HandTypes hand)
  {
    if (timeline.act >= Timeline.Acts.OpenRoof2)
    {
      if (hand == Handedness.HandTypes.HandLeft)
      {
        return act4MovementsLeft.UnwrapOrLoad().RandomElement();
      }
      else
      {
        return act4MovementsRight.UnwrapOrLoad().RandomElement();
      }
    }
    else if (timeline.act >= Timeline.Acts.ArmsToShoulderPlayerDifferentActions)
    {
      if (hand == Handedness.HandTypes.HandLeft)
      {
        return act3Movements.UnwrapOrLoad().GetValue(Act3Movements.replacementShoulderLeft).Unwrap();
      }
      else
      {
        return act3Movements.UnwrapOrLoad().GetValue(Act3Movements.replacementShoulderRight).Unwrap();

      }
    }
    else if (timeline.act >= Timeline.Acts.ArmsDoingDifferentActions)
    {
      if (hand == Handedness.HandTypes.HandLeft)
      {
        return act2EndMovementsLeft.UnwrapOrLoad().RandomElement();
      }
      else
      {
        return act2EndMovementsRight.UnwrapOrLoad().RandomElement();
      }
    }

    else if (timeline.act >= Timeline.Acts.SpawnArmsOnPlatform)
    {
      if (hand == Handedness.HandTypes.HandLeft)
      {
        return act2StartMovementsLeft.UnwrapOrLoad().RandomElement();
      }
      else
      {
        return act2StartMovementsRight.UnwrapOrLoad().RandomElement();
      }
    }
    throw new NullReferenceException("Cannot get random movement");
  }
}
