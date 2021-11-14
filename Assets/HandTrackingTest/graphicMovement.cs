using UnityEngine;

public class graphicMovement : MonoBehaviour
{
  public GameObject Camera;

  public float lerpSpeed = 0.98f;

  // Update is called once per frame
  void Update()
  {
    transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, transform.localPosition, lerpSpeed);
    transform.localRotation = Quaternion.Slerp(Camera.transform.localRotation, transform.localRotation, lerpSpeed);
    var eulers = transform.localRotation.eulerAngles;
    transform.localRotation = Quaternion.Euler(eulers.x, eulers.y, 0);
  }
}
