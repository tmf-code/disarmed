using UnityEngine;

partial class Solve3D
{
  public struct Link
  {
    /**
 * The rotation at the base of the link
 */
    public readonly Quaternion rotation;
    public readonly EulerConstraint constraints;
    public readonly Vector3 position;
    public Link(
        Quaternion rotation,
        EulerConstraint constraints,
        Vector3 position
    )
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
        out EulerConstraint constraints,
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
          CopyConstraints(constraints),
          position
      );
    }
  }
}


