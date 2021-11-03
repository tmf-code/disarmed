using UnityEngine;

public class LightingController : MonoBehaviour
{
  public LightingState state = LightingState.Dark;
  private LightingState _state = LightingState.Dark;
  public Light directional;
  public Light spot;
  public Option<SimpleAnimation> maybeAnimation = new None<SimpleAnimation>();

  public float nextIntensity;
  public float previousIntensity;
  private readonly Color darkLight = new Color(0, 0, 0, 1F);
  private readonly Color dimLight = new Color(0, 0, 0.1F, 1F);
  private readonly Color blueLight = Color.blue;

  public Color nextColor;
  public Color previousColor;

  public enum LightingState
  {
    Dark,
    Light,
    Dim,
  }


  void SetLightingState()
  {
    maybeAnimation = new Some<SimpleAnimation>(new SimpleAnimation(3F, SimpleAnimation.EasingFunction.EaseInCubic, Time.time));
    previousColor = Camera.main.backgroundColor;
    previousIntensity = directional.intensity;
    switch (_state)
    {
      case LightingState.Dark:
        nextColor = darkLight;
        nextIntensity = 0;

        break;
      case LightingState.Light:

        nextColor = blueLight;
        nextIntensity = 1;

        break;
      case LightingState.Dim:
        nextColor = dimLight;
        nextIntensity = 1;
        break;
    }
  }

  void Start()
  {
    Camera.main.backgroundColor = darkLight;
    directional.intensity = 0;
    spot.intensity = 0;
    nextColor = Camera.main.backgroundColor;
    previousColor = Camera.main.backgroundColor;
    nextIntensity = directional.intensity;
    previousIntensity = directional.intensity;
  }

  public void Update()
  {
    if (_state != state)
    {
      _state = state;
      SetLightingState();
    }

    maybeAnimation.Match(() => { }, animation =>
    {
      animation.Update(Time.time);
      if (animation.progression == 0F || animation.progression == 1F)
      {
        if (animation.progression == 1F) maybeAnimation = new None<SimpleAnimation>();
        return;
      };

      directional.intensity = Mathf.Lerp(previousIntensity, nextIntensity, animation.easedProgression);
      spot.intensity = Mathf.Lerp(previousIntensity, nextIntensity, animation.easedProgression);
      Camera.main.backgroundColor = Color.Lerp(previousColor, nextColor, animation.easedProgression);
    });
  }
}
