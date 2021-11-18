using UnityEngine;

public class LightingController : MonoBehaviour
{
  public LightingState state = LightingState.Dark;
  private LightingState _state = LightingState.Dark;
  public Light point;
  public Light spot;
  public Option<SimpleAnimation> maybeAnimation = new None<SimpleAnimation>();

  public float nextPointIntensity;
  public float nextSpotIntensity;
  public float previousPointInstensity;
  public float previousSpotInstensity;
  private readonly Color darkLight = new Color(0, 0, 0, 1F);
  private readonly Color dimLight = new Color(0.5F, 0, 0.1F, 1F);
  private readonly Color dimBlueLight = new Color(0.0F, 0, 0.1F, 1F);
  private readonly Color blueLight = Color.blue;

  public Color nextColor;
  public Color previousColor;

  public enum LightingState
  {
    Dark,
    SpotOnly,
    Both,
    Dim,
    DimBlueLight,
    DarkEnd,
  }


  void SetLightingState()
  {
    maybeAnimation = new Some<SimpleAnimation>(new SimpleAnimation(3F, SimpleAnimation.EasingFunction.EaseInCubic, Time.time));
    previousColor = Camera.main.backgroundColor;
    previousPointInstensity = point.intensity;
    previousSpotInstensity = spot.intensity;
    switch (_state)
    {
      case LightingState.Dark:
        nextColor = darkLight;
        nextPointIntensity = 0;
        nextSpotIntensity = 0;

        break;
      case LightingState.SpotOnly:
        nextColor = blueLight;
        nextPointIntensity = 0;
        nextSpotIntensity = 1;

        break;
      case LightingState.Both:
        nextColor = blueLight;
        nextPointIntensity = 0.6F;
        nextSpotIntensity = 1;

        break;
      case LightingState.Dim:
        nextColor = dimLight;
        nextPointIntensity = 0.1F;
        nextSpotIntensity = 0.6F;
        break;
      case LightingState.DimBlueLight:
        nextColor = dimBlueLight;
        nextPointIntensity = 0.1F;
        nextSpotIntensity = 0.6F;
        break;
    }
  }

  void Start()
  {
    Camera.main.backgroundColor = darkLight;
    point.intensity = 0;
    spot.intensity = 0;
    nextColor = Camera.main.backgroundColor;
    previousColor = Camera.main.backgroundColor;
    nextPointIntensity = point.intensity;
    nextSpotIntensity = spot.intensity;
    previousPointInstensity = point.intensity;
    nextSpotIntensity = spot.intensity;
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

      point.intensity = Mathf.Lerp(previousPointInstensity, nextPointIntensity, animation.easedProgression);
      spot.intensity = Mathf.Lerp(previousSpotInstensity, nextSpotIntensity, animation.easedProgression);
      Camera.main.backgroundColor = Color.Lerp(previousColor, nextColor, animation.easedProgression);

      if (_state == LightingState.DarkEnd)
      {
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Camera.main.backgroundColor, animation.easedProgression);
      }
    });
  }
}
