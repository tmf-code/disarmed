using System;
using UnityEngine;

public class ReleaseGrabbingIntent : MonoBehaviour, IntentMethods, PairedAction
{
  public ReleaseGrabbedIntent other;
  PairedAction PairedAction.other { get => other; }
  public Func<bool> isHandOpen;
  public float collisionDuration = 0;
  public float timeToSuccess = 3;
  public GameObject indicatorObject;

  void Start()
  {
    var maybePrefab = GameObject.Find("Player").GetComponent<IndicatorPrefab>().indicator;
    indicatorObject = Instantiate(maybePrefab);
    var indicator = indicatorObject.GetComponent<Indicator>();
    indicator.target = gameObject.GetComponent<ChildDictionary>().model;
    indicator.progression = () => collisionDuration / timeToSuccess;
  }

  void Update()
  {
    if (!isHandOpen())
    {
      OnIntentCancelled();
      return;
    }

    collisionDuration += Time.deltaTime;
    if (collisionDuration > timeToSuccess) OnIntentComplete();
  }

  void OnDestroy()
  {
    Destroy(indicatorObject, 0.2F);
  }


  public void OnIntentCancelled()
  {
    indicatorObject.GetComponent<Indicator>().FadeOutAudio();
    GrabOperations.CancelReleaseIntent(this, other);
  }

  public void OnIntentComplete()
  {
    GrabOperations.ReleaseIntentToAwaiting(this, other);
  }
}


