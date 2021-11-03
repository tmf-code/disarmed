using UnityEngine;
using UnityEngine.UI;

public class TextCanvas : MonoBehaviour
{

  private new Camera camera;

  public enum TextTypes
  {
    Static,
    Lerp
  }

  public enum Acts
  {
    Act1,
    Act2,
    Act3,
    Act4
  }

  public Acts act = Acts.Act1;
  public TextTypes type = TextTypes.Lerp;
  public Text actNumber;
  public Text actTitle;
  public Color transparent;
  public Color white;

  private bool _blendOut = false;


  [HideInInspector] public SimpleAnimation opacityAnimation;
  public float lerpSpeed = 0.98f;
  public bool setBlendOut = false;
  public bool blendOut
  {
    get => _blendOut; set
    {
      if (value != _blendOut)
      {
        opacityAnimation = new SimpleAnimation(1F, SimpleAnimation.EasingFunction.EaseOutCubic, Time.time);
      }
      _blendOut = value;
    }
  }

  void Start()
  {
    camera = Camera.main;
    actNumber.color = transparent;
    actTitle.color = transparent;
    opacityAnimation = new SimpleAnimation(1F, SimpleAnimation.EasingFunction.EaseOutCubic, Time.time);
  }

  void OnEnable()
  {
    blendOut = false;
  }

  public void SetAct(Acts act)
  {
    switch (act)
    {
      case Acts.Act1:
        actNumber.text = "Act 1.";
        actTitle.text = "Pointed Dreams of a Pair of Arms";
        break;
      case Acts.Act2:
        actNumber.text = "Act 2.";
        actTitle.text = "The Missing Touch";
        break;
      case Acts.Act3:
        actNumber.text = "Act 3.";
        actTitle.text = "Losing a Grip on Control";
        break;
      case Acts.Act4:
        actNumber.text = "Act 4.";
        actTitle.text = "Disarmed";
        break;
    }
  }

  void Update()
  {
    UpdateBlending();

    switch (type)
    {
      case TextTypes.Static:
        break;
      case TextTypes.Lerp:
        transform.localPosition = Vector3.Lerp(transform.localPosition, camera.transform.localPosition, lerpSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, camera.transform.localRotation, lerpSpeed);
        break;
    }
  }

  private void UpdateBlending()
  {
    opacityAnimation.Update(Time.time);

    if (setBlendOut)
    {
      setBlendOut = false;
      blendOut = true;
    }

    if (!blendOut)
    {
      var color = Color.Lerp(transparent, white, opacityAnimation.easedProgression);
      actNumber.color = color;
      actTitle.color = color;
    }
    else
    {
      var color = Color.Lerp(white, transparent, opacityAnimation.easedProgression);
      actNumber.color = color;
      actTitle.color = color;
      if (opacityAnimation.progression == 1.0F)
      {
        gameObject.SetActive(false);
      }
    }
  }
}
