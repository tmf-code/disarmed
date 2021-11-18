using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerArmBehaviour;

public class Timeline : MonoBehaviour
{
  public TextCanvas textCanvas;
  public LightingController lightingController;
  public GameObject ghostHands;
  public GameObject bigArmFromRoof;
  public ArmPool armPool;
  public GameObject largeArmHoldingArms;
  public GameObject platformCollider;

  public AudioPlayer audioPlayer;
  public WorldSceneSelector worldSceneSelector;
  public PlayerArms playerArms;
  public Tracking tracking;

  public enum Acts
  {
    Opening,
    FitPlayersArmsIntoGhost,
    OpeningEnd,

    One,
    WaitForRoofOpen,
    OpenRoof1,
    ArmFromCeiling,
    CloseRoof1,

    DuoArms,
    CopiesOfPlayersArms,

    ShowHighStairs,
    SpawnArmsOnExtraStairs,
    VariableOffset,
    DespawnArmsOnAllStairs,
    HideHighStairs,
    OneEnd,

    Two,
    WallsMoveBack,
    SpawnArmsOnPlatform,
    WavingArmsOnPlatform,
    ArmsTurnToRagdoll,
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
    ThreeEnd,

    Four,
    OpenRoof2,
    LargeArmHoldingArms,
    PlaceArmsInHand,
    ArmRain,
    CloseRoof2,
    PlayersArmsAndMovingArms,
    FloorOpens,
    FourEnd,

    FadeToBlack,
    Credits,
    FogFades,
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
    playerArms.gameObject.GetComponentOrThrow<Rigidbody>().detectCollisions = false;
    playerArms.gameObject.GetComponentOrThrow<Rigidbody>().useGravity = false;


    ActDescription D(float duraction, params GameObject[] array) => new ActDescription(duraction, array.ToList());
    D(0.0F, ghostHands);
    allObjects = L(
      ghostHands,
      bigArmFromRoof,
      largeArmHoldingArms
    );

    actDescriptions = new Dictionary<Acts, ActDescription>((int)Acts.Credits + 1) {
      {Acts.Opening,                                  D(3F)},
      {Acts.FitPlayersArmsIntoGhost,                  D(3F, ghostHands)},
      {Acts.OpeningEnd,                               D(3F)},

      {Acts.One,                                      D(3F)},

      {Acts.WaitForRoofOpen,                          D(52F)},
      {Acts.OpenRoof1,                                D(2F)},
      {Acts.ArmFromCeiling,                           D(110F, bigArmFromRoof)},
      {Acts.CloseRoof1,                               D(2F)},

      {Acts.DuoArms,                                  D(34.5F)},
      {Acts.CopiesOfPlayersArms,                      D(28.5F)},
      {Acts.ShowHighStairs,                           D(5F)},
      {Acts.SpawnArmsOnExtraStairs,                   D(3F)},
      {Acts.VariableOffset,                           D(65F)},
      {Acts.DespawnArmsOnAllStairs,                   D(1F)},
      {Acts.HideHighStairs,                           D(8F)},
      {Acts.OneEnd,                                   D(3F)},

      {Acts.Two,                                      D(3F)},
      {Acts.WallsMoveBack,                            D(8F)},
      {Acts.SpawnArmsOnPlatform,                      D(3F)},
      {Acts.WavingArmsOnPlatform,                     D(22F)},
      {Acts.ArmsTurnToRagdoll,                        D(3F)},

      {Acts.WallsContract,                            D(8F)},
      {Acts.InactiveRagdollArmsInCenter,              D(36F)},
      {Acts.RemoveArms1,                              D(1F)},
      {Acts.RemoveArms2,                              D(2F)},
      {Acts.RemoveArms3,                              D(2F)},
      {Acts.ArmsDoingDifferentActions,                D(35F)},
      {Acts.TwoEnd,                                   D(3F)},

      {Acts.Three,                                    D(3F)},
      {Acts.LeftArmRightArmSwapped,                   D(28F)},
      {Acts.ArmsDropToFloor,                          D(30F)},
      {Acts.ArmsToShoulderPlayerDifferentActions,     D(12F)},
      {Acts.ThreeEnd,                                 D(3F)},

      {Acts.Four,                                     D(3F)},
      {Acts.OpenRoof2,                                D(2F)},
      {Acts.LargeArmHoldingArms,                      D(2F, largeArmHoldingArms)},
      {Acts.PlaceArmsInHand,                          D(11F, largeArmHoldingArms)},
      {Acts.ArmRain,                                  D(24F)},
      {Acts.CloseRoof2,                               D(2F)},
      {Acts.PlayersArmsAndMovingArms,                 D(16F)},
      {Acts.FloorOpens,                               D(6F)},
      {Acts.FourEnd,                                  D(10F)},
      {Acts.FadeToBlack,                              D(3F)},
      {Acts.Credits,                                  D(10F)},
      {Acts.FogFades,                                 D(10F)},
    };

    coroutine = NextAct();
    StartCoroutine(coroutine);
  }

  public void Stop()
  {
    state = State.Stopped;
    StopCoroutine(coroutine);
  }

  void OnDisable()
  {
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
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserInput;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserInput;
          armPool.SetStairState(ArmPool.StairState.None);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Tuning);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.Idle);
          lightingController.state = LightingController.LightingState.Dark;
          textCanvas.state = TextCanvas.TextState.Opaque;
          break;
        case Acts.FitPlayersArmsIntoGhost:
          lightingController.state = LightingController.LightingState.SpotOnly;
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
        case Acts.WaitForRoofOpen:
          textCanvas.state = TextCanvas.TextState.Transparent;
          lightingController.state = LightingController.LightingState.SpotOnly;
          break;

        case Acts.OpenRoof1:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.OpenRoof);
          break;
        case Acts.ArmFromCeiling:
          bigArmFromRoof.GetComponentInChildren<Animator>().Play("LowerArm", 0);
          break;
        case Acts.CloseRoof1:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.CloseRoof);
          break;

        case Acts.DuoArms:
          lightingController.state = LightingController.LightingState.Both;
          armPool.SetStairState(ArmPool.StairState.DuoArms);

          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          break;
        case Acts.CopiesOfPlayersArms:
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserInput;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserInput;
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          armPool.SetStairState(ArmPool.StairState.TwoCopy);
          break;

        case Acts.ShowHighStairs:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ShowHighStairs);
          break;
        case Acts.SpawnArmsOnExtraStairs:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          armPool.SetStairState(ArmPool.StairState.All);
          break;
        case Acts.VariableOffset:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          armPool.SetStairState(ArmPool.StairState.VariableOffset);
          break;
        case Acts.DespawnArmsOnAllStairs:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          armPool.SetStairState(ArmPool.StairState.None);
          break;
        case Acts.HideHighStairs:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.HideHighStairs);
          break;

        case Acts.OneEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Two:
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act2);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act2;
          break;

        case Acts.WallsMoveBack:
          textCanvas.state = TextCanvas.TextState.Transparent;
          lightingController.state = LightingController.LightingState.PartySolid;
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.ExtendRoom);
          break;

        case Acts.SpawnArmsOnPlatform:
          lightingController.state = LightingController.LightingState.PartyPulse;

          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          armPool.SetStairState(ArmPool.StairState.Flat);
          break;

        case Acts.WavingArmsOnPlatform:
          // To enjoy
          break;
        case Acts.ArmsTurnToRagdoll:
          lightingController.state = LightingController.LightingState.PartySolid;
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserCollideWithArms;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserCollideWithArms;
          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserCollideWithArms);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserCollideWithArms);
          armPool.SetStairState(ArmPool.StairState.FlatRagdoll);
          break;
        case Acts.WallsContract:
          lightingController.state = LightingController.LightingState.Both;
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
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserInput;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserInput;

          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);

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
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserInput;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserInput;

          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserInput);
          break;
        case Acts.LeftArmRightArmSwapped:
          tracking.SetSwapped(true);
          lightingController.state = LightingController.LightingState.Both;
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;
        case Acts.ArmsDropToFloor:
          tracking.SetSwapped(false);
          playerArms.left.behaviour = PlayerArmBehaviours.ResponsiveRagdoll;
          playerArms.right.behaviour = PlayerArmBehaviours.ResponsiveRagdoll;
          break;
        case Acts.ArmsToShoulderPlayerDifferentActions:
          playerArms.left.behaviour = PlayerArmBehaviours.MovementPlaybackArmSocket;
          playerArms.right.behaviour = PlayerArmBehaviours.MovementPlaybackArmSocket;
          break;
        case Acts.ThreeEnd:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Four:
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act4);
          audioPlayer.gameObject.transform.parent = playerArms.transform;
          playerArms.left.behaviour = PlayerArmBehaviours.TrackUserCollideWithArms;
          playerArms.right.behaviour = PlayerArmBehaviours.TrackUserCollideWithArms;

          playerArms.left.UpdateBehaviour(PlayerArmBehaviours.TrackUserCollideWithArms);
          playerArms.right.UpdateBehaviour(PlayerArmBehaviours.TrackUserCollideWithArms);
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Act4;
          break;
        case Acts.OpenRoof2:
          lightingController.state = LightingController.LightingState.DimBlueLight;
          textCanvas.state = TextCanvas.TextState.Transparent;
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.OpenRoof);
          audioPlayer.PlayAct(AudioPlayer.ActAudio.Act4Ticking);
          break;
        case Acts.LargeArmHoldingArms:
          largeArmHoldingArms.GetComponentInChildren<Animator>().Play("ArmDrop", 0);
          break;
        case Acts.PlaceArmsInHand:
          armPool.SetStairState(ArmPool.StairState.Act4);
          break;
        case Acts.ArmRain:
          armPool.SetStairState(ArmPool.StairState.Act4ArmRain);
          break;
        case Acts.CloseRoof2:
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.CloseRoof);
          break;
        case Acts.PlayersArmsAndMovingArms:
          break;
        case Acts.FloorOpens:
          Physics.gravity = Vector3.down * 0.1F;
          platformCollider.SetActive(false);
          worldSceneSelector.ChangeScene(WorldSceneSelector.WorldScene.OpenFloor);
          playerArms.gameObject.GetComponentOrThrow<Rigidbody>().detectCollisions = true;
          playerArms.gameObject.GetComponentOrThrow<Rigidbody>().useGravity = true;
          break;
        case Acts.FourEnd:
          textCanvas.state = TextCanvas.TextState.Transparent;
          break;

        case Acts.FadeToBlack:
          lightingController.state = LightingController.LightingState.Dark;
          break;

        case Acts.Credits:
          textCanvas.state = TextCanvas.TextState.Opaque;
          textCanvas.act = TextCanvas.Acts.Credits;
          break;
        case Acts.FogFades:
          // lightingController.state = LightingController.LightingState.DarkEnd;
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
        if (act == Acts.FogFades)
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