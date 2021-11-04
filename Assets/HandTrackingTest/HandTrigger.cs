using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{

  public GameObject Arm;
  private Renderer armRenderer;

  private AudioSource sound;

  private float colorCounter = 0f;

  public GameObject triggerObject;

  private Color normalColor;

  // Start is called before the first frame update
  void Start()
  {
    armRenderer = Arm.GetComponent<Renderer>();
    sound = GetComponent<AudioSource>();
    sound.Stop();
    normalColor = armRenderer.material.color;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == triggerObject)
    {
      sound.Play();
    }
  }

  private void OnTriggerStay(Collider other)
  {
    if (other.gameObject == triggerObject)
    {
      if (colorCounter < 1f)
      {
        colorCounter += Time.deltaTime * 0.2f;
        armRenderer.material.SetColor("_Color", Color.Lerp(normalColor, Color.white, colorCounter));
      }
      else
      {
        sound.enabled = false;
        //What happens when Arm is full
        armRenderer.material.SetColor("_Color", Color.white);

        this.transform.SetParent(other.transform);
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject == triggerObject)
    {
      armRenderer.material.SetColor("_Color", normalColor);
      colorCounter = 0f;
      sound.Stop();
    }
  }
}
