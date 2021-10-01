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
    target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
  }
  void OnTriggerExit(Collider collider)
  {
    target.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
  }
  void OnTriggerStay(Collider collider)
  {
    target.SendMessage("OnTriggerStay", collider, SendMessageOptions.DontRequireReceiver);
  }
}
