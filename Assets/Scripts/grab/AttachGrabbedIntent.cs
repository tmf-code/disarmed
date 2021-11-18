using UnityEngine;

public class AttachGrabbedIntent : MonoBehaviour, PairedAction
{
  public AttachGrabbingIntent other;
  PairedAction PairedAction.other { get => other; }

  private new SimpleAnimation animation;
  public Quaternion selectedStrategy;

  void Start()
  {
    animation = new SimpleAnimation(3, SimpleAnimation.EasingFunction.Linear, Time.time);
  }

  void Update()
  {
    var targetTransform = Grabbed.GetTargetTransform(other.gameObject);
    animation.Update(Time.time);

    var currentTransform = transform.FindRecursiveOrThrow("Model");

    var position = Vector3.LerpUnclamped(
      currentTransform.position,
      targetTransform.position,
      animation.progression);

    var rotation = Quaternion.SlerpUnclamped(
      currentTransform.rotation,
      targetTransform.rotation * selectedStrategy,
      animation.progression);

    currentTransform.SetPositionAndRotation(position, rotation);
  }
}


