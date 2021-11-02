using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Pixelplacement;
//using UnityXR;

public class TextMovement : MonoBehaviour
{

  public GameObject Camera;

  public Vector3 startingPosition;

  public float threshold = 60f;

  public int type = 0;

  public float type2DelaySpeed = 0.9f;

  // Start is called before the first frame update
  void Start()
  {
    //this.transform.localPosition = new Vector3(0, Camera.trandform.localPosition.y, 0);
    this.transform.localPosition = startingPosition;
  }

  // Update is called once per frame
  void Update()
  {
    if (type == 0)
    {
      //   float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(Camera.transform.eulerAngles.y, this.transform.eulerAngles.y));

      //   if (deltaAngle > threshold)
      //   {
      //     Tween.Stop(this.transform.GetInstanceID());
      //     Tween.LocalPosition(this.transform, Camera.transform.localPosition, 0.5f, 0, Tween.EaseInOutStrong);
      //     Tween.LocalRotation(this.transform, Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0), 0.5f, 0, Tween.EaseInOutStrong);
      //   }
    }
    else if (type == 1)
    {
      //this.transform.localPosition = new Vector3(0, Camera.transform.localPosition.y, 0);
      //this.transform.localRotation = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);
      this.transform.localPosition = Camera.transform.localPosition;
      this.transform.localRotation = Camera.transform.localRotation;
    }
    else if (type == 2)
    {
      this.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, this.transform.localPosition, type2DelaySpeed);
      this.transform.localRotation = Quaternion.Slerp(Camera.transform.localRotation, this.transform.localRotation, type2DelaySpeed);
    }
    else if (type == 3)
    {
      this.transform.localPosition = startingPosition;
      this.transform.localRotation = Quaternion.identity;
    }
  }
}
