using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Timeline : MonoBehaviour
{
  public TextCanvas textCanvas;
  public LightingController lightingController;
  public GameObject ghostHands;
  public GameObject bigArmFromRoof;
  public GameObject plinthArms;
  public GameObject extraStairs;
  public GameObject largeArmHoldingArms;
  public GameObject wallsMoveBack;

  public AudioPlayer audioPlayer;
  public WorldSceneSelector worldSceneSelector;
  public PlayerArms playerArms;
  // public GameObject wallsContract;
  // public GameObject armsDoingDifferentActions;
  // public GameObject leftArmRightArmSwapped;
  // public GameObject armsDropToFloor;
  // public GameObject armsToShoulderPlayerDifferentActions;
  // public GameObject oneArmTakesOffOther;
  public GameObject tryOnArms;
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
      {Acts.ArmFromCeiling, 75F},
      {Acts.CopiesOfPlayersArms, 75F},
      {Acts.ExtraStairs, 75F},
      {Acts.LargeArmHoldingArms, 75F},
      {Acts.OneEnd, 3F},

      {Acts.Two, 3F},
      {Acts.WallsMoveBack, 4F},
      {Acts.WallsContract, 4F},
      {Acts.AmsDoingDifferentActions, 40F},
      {Acts.TwoEnd, 3F},

      {Acts.Three, 3F},
      {Acts.LeftArmRightArmSwapped, 23F},
      {Acts.ArmsDropToFloor, 23F},
      {Acts.ArmsToShoulderPlayerDifferentActions, 23F},
      {Acts.OneArmTakesOffOther, 23F},
      {Acts.ThreeEnd, 3F},

      {Acts.Four, 3F},
      {Acts.PlayersArmsAndMovingArms, 60F},
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
      ghostHands,
      bigArmFromRoof,
      plinthArms,
      extraStairs,
      largeArmHoldingArms,
      wallsMoveBack,
      tryOnArms
    );

    activeObjectPerStage = new Dictionary<Acts, List<GameObject>>((int)Acts.End + 1) {
      {Acts.Opening, L()},
      {Acts.FitPlayersArmsIntoGhost, L(ghostHands)},
      {Acts.OpeningEnd, L()},

      {Acts.One, L()},
      {Acts.ArmFromCeiling, L(bigArmFromRoof)},
      {Acts.CopiesOfPlayersArms, L(plinthArms)},
      {Acts.ExtraStairs, L(plinthArms, extraStairs)},
      {Acts.LargeArmHoldingArms, L(largeArmHoldingArms)},
      {Acts.OneEnd, L()},

      {Acts.Two, L(wallsMoveBack)},
      {Acts.WallsMoveBack, L(wallsMoveBack)},
      {Acts.WallsContract, L(wallsMoveBack)},
      {Acts.AmsDoingDifferentActions, L(wallsMoveBack, plinthArms)},
      {Acts.TwoEnd, L()},

      {Acts.Three, L()},
      {Acts.LeftArmRightArmSwapped, L()},
      {Acts.ArmsDropToFloor, L()},
      {Acts.ArmsToShoulderPlayerDifferentActions, L()},
      {Acts.OneArmTakesOffOther, L()},
      {Acts.ThreeEnd, L()},

      {Acts.Four, L(tryOnArms)},
      {Acts.PlayersArmsAndMovingArms, L(tryOnArms)},
      {Acts.AllArmsWaveGoodbye, L(tryOnArms)},
      {Acts.FourEnd, L(tryOnArms)},

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
          audioPlayer.PlayAct(AudioPlayer.ClipType.Intro);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.Base);
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
          audioPlayer.PlayAct(AudioPlayer.ClipType.Act1);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act1;
          break;
        case Acts.ArmFromCeiling:
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.CopiesOfPlayersArms:
          plinthArms.GetComponentOrThrow<PlinthArms>().SetArmCount(PlinthArms.PlinthArmCount.All);
          break;
        case Acts.ExtraStairs:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExpandRoom);
          break;
        case Acts.LargeArmHoldingArms:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ShrinkRoom);

          break;
        case Acts.OneEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Two:
          audioPlayer.PlayAct(AudioPlayer.ClipType.Act2);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act2;
          break;
        case Acts.WallsMoveBack:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.RevealPlatform);
          lightingController.state = LightingController.LightingState.Dim;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.WallsContract:
          foreach (Transform child in wallsMoveBack.transform)
          {
            if (child.gameObject.TryGetComponent<ArmBehaviour>(out var behaviour))
            {
              behaviour.behavior = ArmBehaviour.ArmBehaviorType.MovementPlaybackRagdoll;
            }
            if (child.gameObject.TryGetComponent<PivotPoint>(out var pivot))
            {
              pivot.pivotPointType = PivotPoint.PivotPointType.Wrist;
            }
          }
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.HidePlatform);
          break;
        case Acts.AmsDoingDifferentActions:
          plinthArms.GetComponentOrThrow<PlinthArms>().SetArmCount(PlinthArms.PlinthArmCount.Eleven);
          break;
        case Acts.TwoEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Three:
          audioPlayer.PlayAct(AudioPlayer.ClipType.Act3);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act3;
          break;
        case Acts.LeftArmRightArmSwapped:
          playerArms.left.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.SwapArms;
          playerArms.right.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.SwapArms;
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmsDropToFloor:
          playerArms.left.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.ResponsiveRagdoll;
          playerArms.right.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.ResponsiveRagdoll;
          break;
        case Acts.ArmsToShoulderPlayerDifferentActions:
          break;
        case Acts.OneArmTakesOffOther:
          break;
        case Acts.ThreeEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Four:
          audioPlayer.PlayAct(AudioPlayer.ClipType.Act4);
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

      var waitTime = durations[act];

      if (act == Acts.FitPlayersArmsIntoGhost)
      {
        var isHoverComplete = ghostHands.GetComponentOrThrow<BothHandsHoverTrigger>().isHoverComplete;
        if (!isHoverComplete)
        {
          yield return new WaitUntil(() => ghostHands.GetComponentOrThrow<BothHandsHoverTrigger>().isHoverComplete);
        }
        else
        {
          yield return new WaitForSeconds(waitTime);
          act += 1;
        }
      }
      else
      {
        if (act == Acts.End)
        {
          Stop();
          yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        act += 1;
      }
    }
  }
}
