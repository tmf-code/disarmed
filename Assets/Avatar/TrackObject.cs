using UnityEngine;

public class TrackObject : MonoBehaviour
{
  public Transform trackObject;

  // Update is called once per frame
  void Update()
  {
    transform.SetPositionAndRotation(trackObject.position, trackObject.rotation);
  }
}
