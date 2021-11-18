using UnityEngine;

public class GrabbedIntent : MonoBehaviour, PairedAction
{
  public GrabbingIntent other;
  PairedAction PairedAction.other { get => other; }
}


