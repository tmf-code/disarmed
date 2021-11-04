using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{

  public GameObject Arm;
  private Renderer armRenderer;


  private AudioSource[] sounds;
  private AudioSource progressionSound;
  private AudioSource passedSound;

  private float colorCounter = 0f;

  private bool passed = false;

  public GameObject triggerObject;

  private Color normalColor;

  // Start is called before the first frame update
  void Start()
  {
    armRenderer = Arm.GetComponent<Renderer>();
    sounds = GetComponents<AudioSource>();
    progressionSound = sounds[0];
    passedSound = sounds[1];

    progressionSound.Stop();
    passedSound.Stop();
    normalColor = armRenderer.material.color;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == triggerObject)
    {
      progressionSound.Play();
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
        if (!passed)
        {
          this.transform.SetParent(other.transform);
          progressionSound.enabled = false;
          passedSound.Play();
          armRenderer.material.SetColor("_Color", Color.white);
        }

      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject == triggerObject)
    {
      armRenderer.material.SetColor("_Color", normalColor);
      colorCounter = 0f;
      progressionSound.Stop();
    }
  }
}
