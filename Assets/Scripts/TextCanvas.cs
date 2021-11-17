using UnityEngine;
using UnityEngine.UI;

public class TextCanvas : MonoBehaviour
{

  private new Camera camera;
  public SpriteRenderer introTitle;
  public SpriteRenderer credits;
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
    Intro,
    Act1,
    Act2,
    Act3,
    Act4,
    Credits
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
    actNumber.text = "";
    actTitle.text = "";
    SetTextState();
    introTitle.color = white;
    credits.color = white;
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
      case Acts.Intro:
        type = TextTypes.Static;
        introTitle.gameObject.SetActive(true);
        credits.gameObject.SetActive(false);
        actNumber.text = "";
        actTitle.text = "";
        break;
      case Acts.Act1:
        type = TextTypes.Static;
        introTitle.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        actNumber.text = "Act 1.";
        actTitle.text = "Pointed Dreams of a Pair of Arms";
        break;
      case Acts.Act2:
        type = TextTypes.Static;
        introTitle.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        actNumber.text = "Act 2.";
        actTitle.text = "The Missing Touch";
        break;
      case Acts.Act3:
        type = TextTypes.Static;
        introTitle.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        actNumber.text = "Act 3.";
        actTitle.text = "Losing a Grip on Control";
        break;
      case Acts.Act4:
        type = TextTypes.Static;
        introTitle.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        actNumber.text = "Act 4.";
        actTitle.text = "Disarmed";
        break;
      case Acts.Credits:
        type = TextTypes.Lerp;
        introTitle.gameObject.SetActive(false);
        credits.gameObject.SetActive(true);
        actNumber.text = "";
        actTitle.text = "";
        break;
    }
  }

  void LateUpdate()
  {
    UpdateBlending();
    UpdateAct();

    switch (type)
    {
      case TextTypes.Static:
        break;
      case TextTypes.Lerp:
        transform.position = Vector3.Lerp(transform.position, camera.transform.position, lerpSpeed);
        transform.position = new Vector3(transform.position.x, camera.transform.position.y, transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, camera.transform.rotation, lerpSpeed);
        var eulers = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulers.x, eulers.y, 0);
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
      var color = Color.Lerp(previousColor, nextColor, animation.easedProgression);
      actTitle.color = color;
      actNumber.color = color;
      introTitle.color = color;
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
