using UnityEngine;

public class Grab : MonoBehaviour
{
  void Start()
  {
    gameObject.AddIfNotExisting<Idle>();
  }
}
