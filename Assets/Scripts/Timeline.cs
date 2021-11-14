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
    OpenRoof1,
    ArmFromCeiling,
    CloseRoof1,
    CopiesOfPlayersArms,

    ShowHighStairs,
    SpawnArmsOnExtraStairs,
    VariableOffset,
    LongTimeOffset,
    DespawnArmsOnAllStairs,
    HideHighStairs,

    OpenRoof2,
    LargeArmHoldingArms,
    CloseRoof2,
    OneEnd,

    Two,
    SpawnArmsOnPlatform,
    WallsMoveBack,
    WavingArmsOnPlatform,
    WallsContract,
    InactiveRagdollArmsInCenter,

    RemoveArms1,
    RemoveArms2,
    RemoveArms3,

    ArmsDoingDifferentActions,
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

  public IReadOnlyDictionary<Acts, ActDescription> actDescriptions;
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
    List<GameObject> L(params GameObject[] array) => array.ToList();

    ActDescription D(float duraction, params GameObject[] array) => new ActDescription(duraction, array.ToList());
    D(0.0F, ghostHands);
    allObjects = L(
      ghostHands,
      bigArmFromRoof,
      largeArmHoldingArms
    );

    actDescriptions = new Dictionary<Acts, ActDescription>((int)Acts.End + 1) {
      {Acts.Opening,                                  D(3F)},
      {Acts.FitPlayersArmsIntoGhost,                  D(3F, ghostHands)},
      {Acts.OpeningEnd,                               D(3F)},

      {Acts.One,                                      D(3F)},

      {Acts.OpenRoof1,                                D(2F)},
      {Acts.ArmFromCeiling,                           D(120F, bigArmFromRoof)},
      {Acts.CloseRoof1,                               D(2F)},

      {Acts.CopiesOfPlayersArms,                      D(20F)},

      {Acts.ShowHighStairs,                           D(5F)},
      {Acts.SpawnArmsOnExtraStairs,                   D(3F)},
      {Acts.VariableOffset,                           D(26F)},
      {Acts.LongTimeOffset,                           D(24F)},
      {Acts.DespawnArmsOnAllStairs,                   D(1F)},
      {Acts.HideHighStairs,                           D(5F)},

      {Acts.OpenRoof2,                                D(2F)},
      {Acts.LargeArmHoldingArms,                      D(28F, largeArmHoldingArms)},
      {Acts.CloseRoof2,                               D(2F)},
      {Acts.OneEnd,                                   D(3F)},

      {Acts.Two,                                      D(3F)},
      {Acts.SpawnArmsOnPlatform,                      D(3F)},
      {Acts.WallsMoveBack,                            D(5F)},
      {Acts.WavingArmsOnPlatform,                     D(25F)},
      {Acts.WallsContract,                            D(5F)},
      {Acts.InactiveRagdollArmsInCenter,              D(37F)},
      {Acts.RemoveArms1,                              D(3F)},
      {Acts.RemoveArms2,                              D(3F)},
      {Acts.RemoveArms3,                              D(3F)},
      {Acts.ArmsDoingDifferentActions,                D(29F)},
      {Acts.TwoEnd,                                   D(3F)},

      {Acts.Three,                                    D(3F)},
      {Acts.LeftArmRightArmSwapped,                   D(27F)},
      {Acts.ArmsDropToFloor,                          D(30F)},
      {Acts.ArmsToShoulderPlayerDifferentActions,     D(36F)},
      {Acts.OneArmTakesOffOther,                      D(23F)},
      {Acts.ThreeEnd,                                 D(3F)},

      {Acts.Four,                                     D(3F)},
      {Acts.PlayersArmsAndMovingArms,                 D(60F)},
      {Acts.AllArmsWaveGoodbye,                       D(3F)},
      {Acts.FourEnd,                                  D(3F)},

      {Acts.End,                                      D(3F)},
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
      actDescriptions.TryGetValue(act, out var activeObjects);
      allObjects.ForEach(testObject => testObject.SetActive(activeObjects.gameObjects.Contains(testObject)));

      switch (act)
      {
        case Acts.Opening:
          armPool.SetStairState(ArmPool.StairState.None);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Tuning);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.Idle);
          lightingController.state = LightingController.LightingState.Dark;
          textCanvas.state = TextCanvas.TextState.Opaque;
          break;
        case Acts.FitPlayersArmsIntoGhost:
          lightingController.state = LightingController.LightingState.Light;
          break;
        case Acts.OpeningEnd:
          lightingController.state = LightingController.LightingState.Dark;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;


        case Acts.One:
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act1);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act1;
          break;

        case Acts.OpenRoof1:
          lightingController.state = LightingController.LightingState.Light;
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.OpenRoof);
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmFromCeiling:
          break;
        case Acts.CloseRoof1:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.CloseRoof);
          break;

        case Acts.CopiesOfPlayersArms:
          armPool.SetStairState(ArmPool.StairState.TwoCopy);
          break;

        case Acts.ShowHighStairs:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ShowHighStairs);
          break;
        case Acts.SpawnArmsOnExtraStairs:
          armPool.SetStairState(ArmPool.StairState.All);
          break;
        case Acts.VariableOffset:
          armPool.SetStairState(ArmPool.StairState.VariableOffset);
          break;
        case Acts.LongTimeOffset:
          armPool.SetStairState(ArmPool.StairState.LongTimeOffset);
          break;
        case Acts.DespawnArmsOnAllStairs:
          armPool.SetStairState(ArmPool.StairState.None);
          break;
        case Acts.HideHighStairs:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.HideHighStairs);
          break;

        case Acts.OpenRoof2:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.OpenRoof);
          break;
        case Acts.LargeArmHoldingArms:
          break;
        case Acts.CloseRoof2:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.CloseRoof);
          break;
        case Acts.OneEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Two:
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act2);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act2;
          break;

        case Acts.SpawnArmsOnPlatform:
          lightingController.state = LightingController.LightingState.Dim;
          textCanvas.state = TextCanvas.TextState.Transparent;
          armPool.SetStairState(ArmPool.StairState.Flat);
          break;
        case Acts.WallsMoveBack:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExtendRoom);
          break;
        case Acts.WavingArmsOnPlatform:
          // To enjoy
          break;
        case Acts.WallsContract:
          armPool.SetStairState(ArmPool.StairState.FlatRagdoll);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ShrinkRoom);
          break;
        case Acts.InactiveRagdollArmsInCenter:
          // To enjoy
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
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act3);
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
          Debug.LogWarning("Arms to shoulder not implemented");
          break;
        case Acts.OneArmTakesOffOther:
          Debug.LogWarning("One arm takes off other not implemented");
          break;
        case Acts.ThreeEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Four:
          armPool.SetStairState(ArmPool.StairState.Act4);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act4);
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

        default:
          throw new System.Exception($"{act} not implemented");
      }

      var waitTime = actDescriptions[act].duration;

      if (act == Acts.FitPlayersArmsIntoGhost)
      {
        var isHoverComplete = ghostHands.GetComponentOrThrow<BothHandsHoverTrigger>().isHoverComplete;
        if (!isHoverComplete)
        {
          yield return new WaitUntil(() => ghostHands.GetComponentOrThrow<BothHandsHoverTrigger>().isHoverComplete);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Chime);
        }
        else
        {
          yield return new WaitForSeconds(waitTime * (debugFast && waitTime > 10F ? 0.1F : 1.0F));
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
        yield return new WaitForSeconds(waitTime * (debugFast && waitTime > 10F ? 0.1F : 1.0F));
        act += 1;
      }
    }
  }
}

public struct ActDescription
{
  public float duration;
  public List<GameObject> gameObjects;

  public ActDescription(float duration, List<GameObject> gameObjects)
  {
    this.duration = duration;
    this.gameObjects = gameObjects;
  }
}