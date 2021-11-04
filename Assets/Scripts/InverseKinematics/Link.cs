using UnityEngine;

partial class Solve3D
{
  public struct Link
  {
    /**
 * The rotation at the base of the link
 */
    private Quaternion _rotation;
    public readonly EulerConstraint constraints;
    public readonly Vector3 position;
    public readonly Quaternion rotation => _rotation;

    public Link(
        Quaternion rotation,
        EulerConstraint constraints,
        Vector3 position
    )
    {
      _rotation = rotation;
      this.constraints = constraints;
      this.position = position;
    }

    public override string ToString()
    {
      return $"{position} {rotation} {constraints}";
    }

    public static void ApplyConstraint(ref Link link)
    {
      var (pitch, yaw, roll) = link.constraints;
      var lowerBound = new Vector3(pitch.min, yaw.min, roll.min);
      var upperBound = new Vector3(pitch.max, yaw.max, roll.max);
      var clampedRotation = QuaternionExtensions.Clamp(link.rotation, lowerBound, upperBound);
      link._rotation = clampedRotation;
    }
  }

}


