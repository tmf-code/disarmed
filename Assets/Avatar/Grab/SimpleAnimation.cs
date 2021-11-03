using System;
using UnityEngine;

[Serializable]
public class SimpleAnimation
{
  public float timeCreated;
  public float duration = 1;
  public float lifetime = 0;
  public float progression = 0;
  public float easedProgression = 0;

  public enum EasingFunction
  {
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
  }

  public EasingFunction easing = EasingFunction.Linear;

  public void Reset(float time)
  {
    timeCreated = time;
  }

  public SimpleAnimation(float duration, EasingFunction easing, float time)
  {
    timeCreated = time;
    this.duration = duration;
    this.easing = easing;
  }

  public void Update(float time)
  {
    lifetime = time - timeCreated;
    progression = Mathf.Clamp(lifetime / duration, 0, 1);
    easedProgression = Ease(easing, progression);
  }

  public static float Ease(EasingFunction easing, float progression)
  {
    return easing switch
    {
      EasingFunction.Linear => progression,
      EasingFunction.EaseInQuad => progression * progression,
      EasingFunction.EaseOutQuad => progression * (2 - progression),
      EasingFunction.EaseInOutQuad => progression < .5 ? 2 * progression * progression : -1 + (4 - 2 * progression) * progression,
      EasingFunction.EaseInCubic => progression * progression * progression,
      EasingFunction.EaseOutCubic => (--progression) * progression * progression + 1,
      EasingFunction.EaseInOutCubic => progression < .5 ? 4 * progression * progression * progression : (progression - 1) * (2 * progression - 2) * (2 * progression - 2) + 1,
      EasingFunction.EaseInQuart => progression * progression * progression * progression,
      EasingFunction.EaseOutQuart => 1 - (--progression) * progression * progression * progression,
      EasingFunction.EaseInOutQuart => progression < .5 ? 8 * progression * progression * progression * progression : 1 - 8 * (--progression) * progression * progression * progression,
      EasingFunction.EaseInQuint => progression * progression * progression * progression * progression,
      EasingFunction.EaseOutQuint => 1 + (--progression) * progression * progression * progression * progression,
      EasingFunction.EaseInOutQuint => progression < .5 ? 16 * progression * progression * progression * progression * progression : 1 + 16 * (--progression) * progression * progression * progression * progression,
      _ => throw new NotImplementedException(),
    };
  }
}
