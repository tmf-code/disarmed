using UnityEngine;

public class WorldSceneSelector : MonoBehaviour
{
  public Animator anim;

  public enum WorldScene
  {
    Idle = 0,
    OpenRoof = 1,
    CloseRoof = 2,
    ShowHighStairs = 3,
    HideHighStairs = 4,
    ExtendRoom = 5,
    ShrinkRoom = 6,
  }

  void Start()
  {
    anim.Play(WorldScene.Idle.ToString(), 0);
  }

  public void ChangeScene(WorldScene scene)
  {
    Debug.Log($"Try to play scene {scene}");
    anim.Play(scene.ToString(), 0);
  }
}