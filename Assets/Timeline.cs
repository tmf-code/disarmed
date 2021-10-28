using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
  public enum Acts
  {
    Opening,
    FitPlayersArmsIntoGhost,

    One,
    ArmFromCeiling,
    CopiesOfPlayersArms,
    ExtraStairs,
    LargeArmHoldingArms,

    Two,
    WallsMoveBack,
    WallsContract,
    AmsDoingDifferentActions,

    Three,
    PlayersArmsOnly,

    Four,
    PlayersArmsAndMovingArms,
    AllArmsWaveGoodbye,

    End,
  }

  public readonly IReadOnlyDictionary<Acts, float> durations = new Dictionary<Acts, float>((int)Acts.End + 1) {
      {Acts.Opening, 1F},
      {Acts.FitPlayersArmsIntoGhost, 1F},

      {Acts.One, 1F},
      {Acts.ArmFromCeiling, 1F},
      {Acts.CopiesOfPlayersArms, 1F},
      {Acts.ExtraStairs, 1F},
      {Acts.LargeArmHoldingArms, 1F},

      {Acts.Two, 1F},
      {Acts.WallsMoveBack, 1F},
      {Acts.WallsContract, 1F},
      {Acts.AmsDoingDifferentActions, 1F},

      {Acts.Three, 1F},
      {Acts.PlayersArmsOnly, 1F},

      {Acts.Four, 1F},
      {Acts.PlayersArmsAndMovingArms, 1F},
      {Acts.AllArmsWaveGoodbye, 1F},

      {Acts.End, 1F},
  };

  public enum State
  {

    Playing,
    Stopped,
  }

  public Acts act = Acts.Opening;
  [ShowOnly] public State state = State.Stopped;

  IEnumerator coroutine;

  // Start is called before the first frame update
  void Start()
  {
    coroutine = NextAct();
    StartCoroutine(coroutine);
  }

  public void Stop()
  {
    state = State.Stopped;
    StopCoroutine(coroutine);
  }

  IEnumerator NextAct()
  {
    state = State.Playing;

    while (state == State.Playing)
    {
      if (act == Acts.End)
      {
        Stop();
        yield return null;
      }

      var waitTime = durations[act];
      yield return new WaitForSeconds(waitTime);
      act += 1;
    }
  }
}
