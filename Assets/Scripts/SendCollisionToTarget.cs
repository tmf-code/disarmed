using UnityEngine;

public class SendCollisionToTarget : MonoBehaviour
{
  public GameObject target;
  void OnCollisionEnter(Collision collision)
  {
    target.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
  }
  void OnCollisionExit(Collision collision)
  {
    target.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);
  }
  void OnCollisionStay(Collision collision)
  {
    target.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
  }
  void OnTriggerEnter(Collider collider)
  {
    if (collider.TryGetComponent<SendCollisionToTarget>(out var otherTarget))
    {
      var colliders = new TwoPartyCollider(GetComponent<CapsuleCollider>(), collider, target, otherTarget.target);
      target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
      target.SendMessage("OnTriggersEnter", colliders, SendMessageOptions.DontRequireReceiver);
    }
  }
}

public class TwoPartyCollider
{
  public Collider source;
  public Collider other;

  public GameObject sourceTarget;
  public GameObject otherTarget;

  public TwoPartyCollider(Collider source, Collider other, GameObject sourceTarget, GameObject otherTarget)
  {
    this.source = source;
    this.other = other;
    this.sourceTarget = sourceTarget;
    this.otherTarget = otherTarget;
  }

  public new string ToString()
  {
    return $"source: {source}, other: {other}";
  }
}