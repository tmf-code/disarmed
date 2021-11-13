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
  public ArmPool armPool;
  public GameObject largeArmHoldingArms;

  public AudioPlayer audioPlayer;
  public WorldSceneSelector worldSceneSelector;
  public PlayerArms playerArms;
  public Tracking tracking;
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
    PlayerArmsOnly,
    ArmFromCeiling,
    CopiesOfPlayersArms,
    ExtraStairs,
    VariableOffset,

    LongTimeOffset,
    LargeArmHoldingArms,
    OneEnd,

    Two,
    WallsMoveBack,
    WallsContract,
    RemoveArms1,
    RemoveArms2,
    RemoveArms3,
    ArmsDoingDifferentActions,
    TwoEnd,

    Three,
    LeftArmRightArmSwapped,
    ArmsDropToFloor,
    ArmsToShoulderPlayerDifferentActions,
    // OneArmTakesOffOther,
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
      {Acts.PlayerArmsOnly, 50F},
      {Acts.ArmFromCeiling, 120F},
      {Acts.CopiesOfPlayersArms, 23F},
      {Acts.ExtraStairs, 3F},
      {Acts.VariableOffset, 30F},
      {Acts.LongTimeOffset, 30F},
      {Acts.LargeArmHoldingArms, 32F},
      {Acts.OneEnd, 3F},

      {Acts.Two, 3F},
      {Acts.WallsMoveBack, 40F},
      {Acts.WallsContract, 40F},
      {Acts.RemoveArms1, 3F},
      {Acts.RemoveArms2, 3F},
      {Acts.RemoveArms3, 3F},
      {Acts.ArmsDoingDifferentActions, 35F},
      {Acts.TwoEnd, 3F},

      {Acts.Three, 3F},
      {Acts.LeftArmRightArmSwapped, 27F},
      {Acts.ArmsDropToFloor, 25F},
      {Acts.ArmsToShoulderPlayerDifferentActions, 35F},
      // {Acts.OneArmTakesOffOther, 23F},
      {Acts.ThreeEnd, 3F},

      {Acts.Four, 3F},
      {Acts.PlayersArmsAndMovingArms, 60F},
      {Acts.AllArmsWaveGoodbye, 3F},
      {Acts.FourEnd, 3F},

      {Acts.End, 3F},
  };

  public IReadOnlyDictionary<Acts, List<GameObject>> activeObjectPerStage;
  [SerializeField] [HideInInspector] private List<GameObject> allObjects;

  public enum State
  {
    Playing,
    Stopped,
  }

  public bool debugFast = false;

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
      largeArmHoldingArms
    );

    activeObjectPerStage = new Dictionary<Acts, List<GameObject>>((int)Acts.End + 1) {
      {Acts.Opening, L()},
      {Acts.FitPlayersArmsIntoGhost, L(ghostHands)},
      {Acts.OpeningEnd, L()},

      {Acts.One, L()},
      {Acts.PlayerArmsOnly, L()},
      {Acts.ArmFromCeiling, L(bigArmFromRoof)},
      {Acts.CopiesOfPlayersArms, L()},
      {Acts.ExtraStairs, L()},
      {Acts.VariableOffset, L()},
      {Acts.LongTimeOffset, L()},
      {Acts.LargeArmHoldingArms, L(largeArmHoldingArms )},
      {Acts.OneEnd, L()},

      {Acts.Two, L()},
      {Acts.WallsMoveBack, L()},
      {Acts.WallsContract, L()},
      {Acts.RemoveArms1, L()},
      {Acts.RemoveArms2, L()},
      {Acts.RemoveArms3, L()},
      {Acts.ArmsDoingDifferentActions, L( )},
      {Acts.TwoEnd, L()},

      {Acts.Three, L()},
      {Acts.LeftArmRightArmSwapped, L()},
      {Acts.ArmsDropToFloor, L()},
      {Acts.ArmsToShoulderPlayerDifferentActions, L()},
      // {Acts.OneArmTakesOffOther, L()},
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
          armPool.SetStairState(ArmPool.StairState.None);

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
        case Acts.PlayerArmsOnly:
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmFromCeiling:
          break;
        case Acts.CopiesOfPlayersArms:
          armPool.SetStairState(ArmPool.StairState.TwoCopy);
          break;
        case Acts.ExtraStairs:
          armPool.SetStairState(ArmPool.StairState.All);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExpandRoom);
          break;
        case Acts.VariableOffset:
          armPool.SetStairState(ArmPool.StairState.VariableOffset);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExpandRoom);
          break;
        case Acts.LongTimeOffset:
          armPool.SetStairState(ArmPool.StairState.LongTimeOffset);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExpandRoom);
          break;
        case Acts.LargeArmHoldingArms:
          armPool.SetStairState(ArmPool.StairState.None);
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
          armPool.SetStairState(ArmPool.StairState.Flat);

          break;
        case Acts.WallsContract:
          armPool.SetStairState(ArmPool.StairState.FlatRagdoll);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.HidePlatform);
          break;
        case Acts.RemoveArms1:
          armPool.SetStairState(ArmPool.StairState.RemoveStepOne);
          break;
        case Acts.RemoveArms2:
          armPool.SetStairState(ArmPool.StairState.RemoveStepTwo);
          break;
        case Acts.RemoveArms3:
          armPool.SetStairState(ArmPool.StairState.RemoveStepThree);
          break;
        case Acts.ArmsDoingDifferentActions:
          armPool.SetStairState(ArmPool.StairState.TwoRecordedMovement);
          break;
        case Acts.TwoEnd:
          armPool.SetStairState(ArmPool.StairState.None);
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Three:
          audioPlayer.PlayAct(AudioPlayer.ClipType.Act3);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act3;
          break;
        case Acts.LeftArmRightArmSwapped:
          tracking.SetSwapped(true);
          lightingController.state = LightingController.LightingState.Light;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmsDropToFloor:
          tracking.SetSwapped(false);
          playerArms.left.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.ResponsiveRagdoll;
          playerArms.right.GetComponentOrThrow<ArmBehaviour>().behavior = ArmBehaviour.ArmBehaviorType.ResponsiveRagdoll;
          break;
        case Acts.ArmsToShoulderPlayerDifferentActions:
          break;
        // case Acts.OneArmTakesOffOther:
        //   break;
        case Acts.ThreeEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Four:
          armPool.SetStairState(ArmPool.StairState.Act4);
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
          yield return new WaitForSeconds(waitTime * (debugFast ? 0.2F : 1.0F));
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
        yield return new WaitForSeconds(waitTime * (debugFast ? 0.2F : 1.0F));
        act += 1;
      }
    }
  }
}
