using UnityEngine;

public class SendCollisionToTarget : MonoBehaviour
{
  public GameObject target;
  public Collider source;

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
    var colliders = new TwoPartyCollider(gameObject.GetComponent<CapsuleCollider>(), collider);
    target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
    target.SendMessage("OnTriggersEnter", colliders, SendMessageOptions.DontRequireReceiver);
  }
  void OnTriggerExit(Collider collider)
  {
    var colliders = new TwoPartyCollider(gameObject.GetComponent<CapsuleCollider>(), collider);
    target.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
    target.SendMessage("OnTriggersExit", colliders, SendMessageOptions.DontRequireReceiver);
  }
  void OnTriggerStay(Collider collider)
  {
    var colliders = new TwoPartyCollider(gameObject.GetComponent<CapsuleCollider>(), collider);
    target.SendMessage("OnTriggersStay", collider, SendMessageOptions.DontRequireReceiver);
    target.SendMessage("OnTriggersStay", colliders, SendMessageOptions.DontRequireReceiver);
  }
}

public class TwoPartyCollider
{
  public Collider source;
  public Collider other;

  public TwoPartyCollider(Collider source, Collider other)
  {
    this.source = source;
    this.other = other;
  }

  public new string ToString()
  {
    return $"source: {source}, other: {other}";
  }
}