using UnityEngine;

public class WorldSceneSelector : MonoBehaviour
{
  private Animator anim;

  public enum WorldScene
  {
    Base = 1,
    ExpandRoom = 2,
    ShrinkRoom = 3,
    RevealPlatform = 4,
    HidePlatform = 5,
  }

  void Start()
  {
    anim = gameObject.GetComponent<Animator>();
  }

  public void ChangeScene(WorldScene scene)
  {
    anim.SetInteger("Scene", (int)scene);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      int currentScene = anim.GetInteger("Scene");
      if (currentScene + 1 < 5)
      {
        anim.SetInteger("Scene", currentScene + 1);
      }
      else
      {
        anim.SetInteger("Scene", 1);
      }
    }
  }
}