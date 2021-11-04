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

  public enum TextState
  {
    Opaque,
    Transparent
  }

  public enum Acts
  {
    Act1,
    Act2,
    Act3,
    Act4
  }

  public Acts act = Acts.Act1;
  private Acts _act = Acts.Act1;
  public TextTypes type = TextTypes.Lerp;
  public Text actNumber;
  public Text actTitle;
  public Color transparent;
  public Color white;
  private TextState _state = TextState.Transparent;
  public TextState state = TextState.Transparent;

  [HideInInspector] public Option<SimpleAnimation> opacityAnimation = new None<SimpleAnimation>();
  private Color previousColor;
  public float lerpSpeed = 0.98f;
  private Color nextColor;

  void Start()
  {
    camera = Camera.main;
    actNumber.color = transparent;
    actTitle.color = transparent;
    actNumber.text = "Act 1.";
    actTitle.text = "Pointed Dreams of a Pair of Arms";
    SetTextState();
  }

  void OnEnable()
  {
    actNumber.color = transparent;
    actTitle.color = transparent;
    previousColor = transparent;
    nextColor = transparent;
    state = TextState.Opaque;
  }

  public void UpdateAct()
  {
    if (_act == act) return;
    _act = act;

    switch (_act)
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
    UpdateAct();

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

  private void SetTextState()
  {
    opacityAnimation = new Some<SimpleAnimation>(new SimpleAnimation(1F, SimpleAnimation.EasingFunction.EaseInCubic, Time.time));
    previousColor = actNumber.color;
    switch (_state)
    {
      case TextState.Transparent:
        nextColor = transparent;
        break;
      case TextState.Opaque:
        nextColor = white;
        break;
    }
  }

  private void UpdateBlending()
  {
    if (_state != state)
    {
      _state = state;
      SetTextState();
    }

    opacityAnimation.Match(() => { }, animation =>
    {
      animation.Update(Time.time);
      actTitle.color = Color.Lerp(previousColor, nextColor, animation.easedProgression);
      actNumber.color = Color.Lerp(previousColor, nextColor, animation.easedProgression);
      if (animation.progression == 0F || animation.progression == 1F)
      {
        if (animation.progression == 1F)
        {
          opacityAnimation = new None<SimpleAnimation>();
        }
        return;
      }
    });
  }
}