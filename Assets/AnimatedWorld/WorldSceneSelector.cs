using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneSelector : MonoBehaviour
{
  private Animator anim;

  void Start()
  {
    anim = gameObject.GetComponent<Animator>();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      // Currently we are using an Animation Controller
      // It will transition whenever the "Scene" parameter changes
      // This is just an integer that we increase until we reach the last scene.
      // We can also just anim.Play("SceneName") instead
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