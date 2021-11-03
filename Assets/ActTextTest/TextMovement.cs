using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;

public class TextMovement : MonoBehaviour
{

  public GameObject Camera;

  public Vector3 startingPosition;

  public float threshold = 60f;

  public enum TextTypes
  {
    AnimateToPosition,
    Static,
    Lerp
  }

  public TextTypes type = TextTypes.Lerp;

  public float lerpSpeed = 0.98f;

  public GameObject ActText;
  public GameObject OpeningText;

  private Text ActTextText;
  private Text OpeningTextText;

  public Color transparent;
  public Color white;

  public bool blendOut = false;

  void Start()
  {
    this.transform.localPosition = startingPosition;

    ActTextText = ActText.GetComponent<Text>();
    OpeningTextText = OpeningText.GetComponent<Text>();

    ActTextText.color = transparent;
    OpeningTextText.color = transparent;

    Tween.Color(ActTextText, white, 0.4f, 0, Tween.EaseOut);
    Tween.Color(OpeningTextText, white, 0.4f, 0, Tween.EaseOut);
  }

  void Update()
  {
    if (type == TextTypes.AnimateToPosition)
    {
      float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(Camera.transform.eulerAngles.y, this.transform.eulerAngles.y));

      if (deltaAngle > threshold)
      {
        Tween.Stop(this.transform.GetInstanceID());
        Tween.LocalPosition(this.transform, Camera.transform.localPosition, 0.5f, 0, Tween.EaseInOutStrong);
        Tween.LocalRotation(this.transform, Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0), 0.5f, 0, Tween.EaseInOutStrong);
      }
    }
    else if (type == TextTypes.Static)
    {
      this.transform.localPosition = Camera.transform.localPosition;
      this.transform.localRotation = Camera.transform.localRotation;
    }
    else if (type == TextTypes.Lerp)
    {
      this.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, this.transform.localPosition, lerpSpeed);
      this.transform.localRotation = Quaternion.Slerp(Camera.transform.localRotation, this.transform.localRotation, lerpSpeed);
    }

    if (blendOut)
    {
      BlendOut();
    }
  }

  public void BlendOut()
  {
    Tween.Color(ActTextText, transparent, 0.2f, 0, Tween.EaseOut);
    Tween.Color(OpeningTextText, transparent, 0.2f, 0, Tween.EaseOut, Tween.LoopType.None, null, Deactivate);
    blendOut = false;
  }

  public void Deactivate()
  {
    gameObject.SetActive(false);
  }
}
