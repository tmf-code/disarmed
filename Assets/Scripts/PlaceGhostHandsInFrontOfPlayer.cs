using UnityEngine;

public class PlaceGhostHandsInFrontOfPlayer : MonoBehaviour
{

  public Camera headset;

  public float lerpSpeed = 0.9f;

  void Start()
  {
    //Debug.Log("Not implemented");
  }

  void Update()
  {
    transform.localPosition = Vector3.Lerp(transform.localPosition, headset.transform.localPosition, 0.5f);
    transform.localRotation = Quaternion.Slerp(transform.localRotation, headset.transform.localRotation, 0.5f);
  }
}
