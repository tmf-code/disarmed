using UnityEngine;

public class ApplyRootRotation : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 1.0F;
  private Transform trackingData;
  private Rigidbody model;

  void Start()
  {
    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model").gameObject.GetComponentOrThrow<Rigidbody>();
  }

  void FixedUpdate()
  {
    var direction = Quaternion.Inverse(model.transform.localRotation) * trackingData.localRotation;
    var rbd = model.GetComponent<Rigidbody>();

    rbd.AddRelativeTorque(new Vector3(direction[0], direction[1], direction[2]) * 500F);
  }
}
