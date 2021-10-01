using UnityEngine;

partial class Solve3D
{
  public struct Link
  {
    /**
 * The rotation at the base of the link
 */
    public readonly Quaternion rotation;

    /**
 * null: No constraint
 *
 * {pitch, yaw, roll}: Range | Number
 *
 * Range: minimum angle, maximum angle (radians), positive is anticlockwise from previous Link's direction vector
 *
 * number: the range of rotation (radian) about the previous links direction vector. A rotation of 90 deg would be 45 deg either direction
 *
 * ExactRotation: Either a global, or local rotation which the Link is locked to
 */
#nullable enable
    public readonly Either<EulerConstraint, ExactRotation>? constraints;
#nullable disable
    public readonly Vector3 position;
#nullable enable
    public Link(
        Quaternion rotation,
        Either<EulerConstraint, ExactRotation>? constraints,
        Vector3 position
    )
#nullable disable

    {
      this.rotation = rotation;
      this.constraints = constraints;
      this.position = position;
    }

    public override string ToString()
    {
      return $"{position} {rotation} {constraints}";
    }

    internal void Deconstruct(
        out Quaternion rotation,
        out Either<EulerConstraint, ExactRotation>? constraints,
        out Vector3 position
    )
    {
      rotation = this.rotation;
      constraints = this.constraints;
      position = this.position;
    }

    public static Link CopyLink(Link link)
    {
      var (rotation, constraints, position) = link;
      return new Link(
          rotation,
          constraints == null ? null : CopyConstraints(constraints),
          position
      );
    }
  }
}


