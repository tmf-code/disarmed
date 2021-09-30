using UnityEngine;

partial class Solve3D
{
  public struct EulerConstraint
  {
    /**
     * Rotation about X
     */
    public readonly Range? pitch;

    /**
     * Rotation about Y
     */
    public readonly Range? yaw;

    /**
     * Rotation about Z
     */
    public readonly Range? roll;


    public EulerConstraint(Range? pitch)
    {
      this.pitch = pitch;
      this.yaw = null;
      roll = null;
    }
    public EulerConstraint(Range? pitch, Range? yaw)
    {
      this.pitch = pitch;
      this.yaw = yaw;
      this.roll = null;
    }

    public EulerConstraint(Range? pitch, Range? yaw, Range? roll)
    {
      this.pitch = pitch;
      this.yaw = yaw;
      this.roll = roll;
    }

    internal void Deconstruct(out Range? pitch, out Range? yaw, out Range? roll)
    {
      pitch = this.pitch;
      yaw = this.yaw;
      roll = this.roll;
    }
  }

  public struct ExactRotation
  {
    public enum ExactRotationType
    {
      GLOBAL = 0,
      LOCAL = 1
    }
    public readonly Quaternion value;
    /**
     * 'local': Relative to previous links direction vector
     *
     * 'global': Relative to the baseJoints world transform
     */
    public readonly ExactRotationType type;

    public ExactRotation(Quaternion value, ExactRotationType type)
    {
      this.value = value;
      this.type = type;
    }
  }

  public static Either<EulerConstraint, ExactRotation> CopyConstraints(Either<EulerConstraint, ExactRotation> constraints)
  {


    return constraints.Match<Either<EulerConstraint, ExactRotation>>(
    Left: constraint => new Left<EulerConstraint, ExactRotation>(
        new EulerConstraint(constraint.pitch, constraint.yaw, constraint.roll)
        ),
    Right: constraint => new Right<EulerConstraint, ExactRotation>(new ExactRotation(constraint.value, constraint.type))
    );
  }
}




