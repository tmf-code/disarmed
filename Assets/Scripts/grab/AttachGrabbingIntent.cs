using System;
using UnityEngine;

public class AttachGrabbingIntent : MonoBehaviour, IntentMethods, PairedAction
{
  public AttachGrabbedIntent other;
  PairedAction PairedAction.other { get => other; }
  public Func<bool> isColliding;
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
    if (!isColliding())
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
    GrabOperations.CancelAttachIntent(this, other);
  }

  public void OnIntentComplete()
  {
    GrabOperations.AttachIntentToAwaiting(this, other);
  }
}


