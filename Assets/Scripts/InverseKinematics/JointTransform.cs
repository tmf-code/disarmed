using UnityEngine;

partial class Solve3D
{
  public struct JointTransform
  {
    public Vector3 position;
    public Quaternion rotation;

    public override string ToString()
    {
      return $"{position} {rotation}";
    }

    public JointTransform(Vector3 position, Quaternion rotation)
    {
      this.position = position;
      this.rotation = rotation;
    }
  }
}


