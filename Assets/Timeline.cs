using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Timeline : MonoBehaviour
{
  public TextCanvas textCanvas;
  public LightingController lightingController;
  // public GameObject ghostHands;
  public GameObject bigArmFromRoof;
  public GameObject copiesOfPlayerArms;
  public GameObject extraStairs;
  public GameObject largeArmHoldingArms;
  public GameObject wallsMoveBack;
  // public GameObject wallsContract;
  // public GameObject armsDoingDifferentActions;
  // public GameObject leftArmRightArmSwapped;
  // public GameObject armsDropToFloor;
  // public GameObject armsToShoulderPlayerDifferentActions;
  // public GameObject oneArmTakesOffOther;
  // public GameObject tryOnArms;
  // public GameObject armsWaveGoodbye;

  public enum Acts
  {
    Opening,
    FitPlayersArmsIntoGhost,
    OpeningEnd,

    One,
    ArmFromCeiling,
    CopiesOfPlayersArms,
    ExtraStairs,
    LargeArmHoldingArms,
    OneEnd,

    Two,
    WallsMoveBack,
    WallsContract,
    AmsDoingDifferentActions,
    TwoEnd,

    Three,
    LeftArmRightArmSwapped,
    ArmsDropToFloor,
    ArmsToShoulderPlayerDifferentActions,
    OneArmTakesOffOther,
    ThreeEnd,

    Four,
    PlayersArmsAndMovingArms,
    AllArmsWaveGoodbye,
    FourEnd,

    End,
  }

  public readonly IReadOnlyDictionary<Acts, float> durations = new Dictionary<Acts, float>((int)Acts.End + 1) {
      {Acts.Opening, 3F},
      {Acts.FitPlayersArmsIntoGhost, 3F},
      {Acts.OpeningEnd, 3F},

      {Acts.One, 3F},
      {Acts.ArmFromCeiling, 3F},
      {Acts.CopiesOfPlayersArms, 3F},
      {Acts.ExtraStairs, 3F},
      {Acts.LargeArmHoldingArms, 3F},
      {Acts.OneEnd, 3F},

      {Acts.Two, 3F},
      {Acts.WallsMoveBack, 3F},
      {Acts.WallsContract, 3F},
      {Acts.AmsDoingDifferentActions, 3F},
      {Acts.TwoEnd, 3F},

      {Acts.Three, 3F},
      {Acts.LeftArmRightArmSwapped, 3F},
      {Acts.ArmsDropToFloor, 3F},
      {Acts.ArmsToShoulderPlayerDifferentActions, 3F},
      {Acts.OneArmTakesOffOther, 3F},
      {Acts.ThreeEnd, 3F},

      {Acts.Four, 3F},
      {Acts.PlayersArmsAndMovingArms, 3F},
      {Acts.AllArmsWaveGoodbye, 3F},
      {Acts.FourEnd, 3F},

      {Acts.End, 3F},
  };

  public IReadOnlyDictionary<Acts, List<GameObject>> activeObjectPerStage;
  public List<GameObject> allObjects;

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
    static List<GameObject> L(params GameObject[] array) => array.ToList();

    allObjects = L(
      bigArmFromRoof,
      copiesOfPlayerArms,
      extraStairs,
      largeArmHoldingArms,
      wallsMoveBack);

    activeObjectPerStage = new Dictionary<Acts, List<GameObject>>((int)Acts.End + 1) {
      {Acts.Opening, L()},
      {Acts.FitPlayersArmsIntoGhost, L()},
      {Acts.OpeningEnd, L()},

      {Acts.One, L()},
      {Acts.ArmFromCeiling, L(bigArmFromRoof)},
      {Acts.CopiesOfPlayersArms, L(copiesOfPlayerArms)},
      {Acts.ExtraStairs, L(copiesOfPlayerArms, extraStairs)},
      {Acts.LargeArmHoldingArms, L(largeArmHoldingArms)},
      {Acts.OneEnd, L()},

      {Acts.Two, L()},
      {Acts.WallsMoveBack, L(wallsMoveBack)},
      {Acts.WallsContract, L()},
      {Acts.AmsDoingDifferentActions, L()},
      {Acts.TwoEnd, L()},

      {Acts.Three, L()},
      {Acts.LeftArmRightArmSwapped, L()},
      {Acts.ArmsDropToFloor, L()},
      {Acts.ArmsToShoulderPlayerDifferentActions, L()},
      {Acts.OneArmTakesOffOther, L()},
      {Acts.ThreeEnd, L()},

      {Acts.Four, L()},
      {Acts.PlayersArmsAndMovingArms, L()},
      {Acts.AllArmsWaveGoodbye, L()},
      {Acts.FourEnd, L()},

      {Acts.End, L()},
  };

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
      activeObjectPerStage.TryGetValue(act, out var activeObjects);
      allObjects.ForEach(testObject => testObject.SetActive(activeObjects.Contains(testObject)));

      switch (act)
      {
        case Acts.Opening:
          lightingController.state = LightingController.LightingState.Dark;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.FitPlayersArmsIntoGhost:
          lightingController.state = LightingController.LightingState.Light;
          break;
        case Acts.OpeningEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.One:
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act1;
          break;
        case Acts.ArmFromCeiling:
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.CopiesOfPlayersArms:
          break;
        case Acts.ExtraStairs:
          break;
        case Acts.LargeArmHoldingArms:
          break;
        case Acts.OneEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Two:
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act2;
          break;
        case Acts.WallsMoveBack:
          lightingController.state = LightingController.LightingState.Dim;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.WallsContract:
          break;
        case Acts.AmsDoingDifferentActions:
          break;
        case Acts.TwoEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Three:
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act3;
          break;
        case Acts.LeftArmRightArmSwapped:
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmsDropToFloor:
          break;
        case Acts.ArmsToShoulderPlayerDifferentActions:
          break;
        case Acts.OneArmTakesOffOther:
          break;
        case Acts.ThreeEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Four:
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act4;
          break;
        case Acts.PlayersArmsAndMovingArms:
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.AllArmsWaveGoodbye:
          break;
        case Acts.FourEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.End:
          break;
      }


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
