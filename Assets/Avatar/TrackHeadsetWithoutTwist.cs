using UnityEngine;

public class TrackHeadsetWithoutTwist : MonoBehaviour
{
  public Transform headset;


  // Update is called once per frame
  void Update()
  {
    transform.position = headset.position;
    var neckDirection = headset.rotation * Vector3.forward;
    var yRotation = Mathf.Atan2(neckDirection.x, neckDirection.z) * Mathf.Rad2Deg;
    transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
  }
}
