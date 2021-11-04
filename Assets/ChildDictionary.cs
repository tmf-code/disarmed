using System;
using UnityEngine;

public class ChildDictionary : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    Debug.Log(String.Join("\n", transform.AllChildren().ConvertAll(child => child.name)));
  }

  // Update is called once per frame
  void Update()
  {

  }
}
