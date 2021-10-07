using System;
using UnityEngine;

public class Grabbed : MonoBehaviour
{
  public Grabbing grabbing;

  public float creationTime;
  public float minimumIdleTimeSeconds = 3;
  public bool canTransition = false;

  private new SimpleAnimation animation;

  void Start()
  {
    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 0;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;


    gameObject.RemoveComponent<Idle>();
    gameObject.RemoveComponent<Grabbing>();

    animation = new SimpleAnimation(3);
  }

  void Update()
  {
    animation.Update();

    var targetTransform = grabbing.transform.FindRecursiveOrThrow("Model");
    var currentTransform = transform.FindRecursiveOrThrow("Model");

    currentTransform.SetPositionAndRotation(
      Vector3.Lerp(
        currentTransform.position,
        targetTransform.position,
        animation.progression
      ),
      Quaternion.Slerp(
        currentTransform.rotation,
        targetTransform.rotation,
        animation.progression
      )
    );

    var currentTime = Time.time - creationTime;
    if (currentTime > minimumIdleTimeSeconds) canTransition = true;
    else canTransition = false;
  }

  public void OnGrabReleased()
  {
    if (!canTransition) return;

    gameObject.GetComponentOrThrow<InverseKinematics>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyHandTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyRootTracking>().strength = 1;
    gameObject.GetComponentOrThrow<ApplyPose>().strength = 0;

    gameObject.AddIfNotExisting<Idle>();

    Destroy(this);
  }

  [Serializable]
  class SimpleAnimation
  {
    public float timeCreated = Time.time;
    public float duration = 1;
    public float lifetime = 0;
    public float progression = 0;

    public SimpleAnimation(float duration)
    {
      this.duration = duration;
    }

    public float Update()
    {
      lifetime = Time.time - timeCreated;
      progression = Mathf.Clamp(lifetime / duration, 0, 1);

      return progression;
    }
  }

}

