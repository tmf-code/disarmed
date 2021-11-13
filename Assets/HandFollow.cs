using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
  public float lerpSpeed = 0.98f;

  public GameObject Camera;

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    this.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, this.transform.localPosition, lerpSpeed);
    this.transform.localRotation = Quaternion.Slerp(Camera.transform.localRotation, this.transform.localRotation, lerpSpeed);
  }
}
