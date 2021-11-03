using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{

  private Renderer cubeRenderer;

  private float colorCounter = 0f;

  // Start is called before the first frame update
  void Start()
  {
    cubeRenderer = GetComponent<Renderer>();
  }

  private void OnTriggerEnter()
  {
    //cubeRenderer.material.SetColor("_Color", Color.green);
  }

  private void OnTriggerStay(Collider other)
  {
    if (other.gameObject.name == "Model R")
    {
      //while the hand is in the box lerp the color from red to blue
      if (colorCounter < 1f)
      {
        colorCounter += Time.deltaTime * 0.25f;
        cubeRenderer.material.SetColor("_Color", Color.Lerp(Color.red, Color.blue, colorCounter));
      }
      else
      {
        //if its done change to green
        cubeRenderer.material.SetColor("_Color", Color.green);

        //and set a new Parent
        this.transform.SetParent(other.transform);
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    //if the hand exits the box change back to red
    cubeRenderer.material.SetColor("_Color", Color.red);
    colorCounter = 0f;
  }
}
