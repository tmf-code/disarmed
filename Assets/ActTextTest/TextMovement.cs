using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityXR;

public class TextMovement : MonoBehaviour
{

    public GameObject Camera;

    public float threshold = 60f;

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.localPosition = new Vector3(0, Camera.trandform.localPosition.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float deltaAngle = Mathf.DeltaAngle(Camera.transform.eulerAngles.y, this.transform.eulerAngles.y);

        if (deltaAngle > threshold){
            this.transform.localPosition = new Vector3(0, Camera.transform.localPosition.y, 0);
            this.transform.localRotation = Camera.transform.localRotation;
        }
    }
}
