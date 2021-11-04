using UnityEngine;

partial class Solve3D
{
  public struct EulerConstraint
  {
    /**
 * Rotation about X
 */
    public readonly Range pitch;

    /**
 * Rotation about Y
 */
    public readonly Range yaw;

    /**
 * Rotation about Z
 */
    public readonly Range roll;

    public EulerConstraint(Range pitch, Range yaw, Range roll)
    {
      this.pitch = pitch;
      this.yaw = yaw;
      this.roll = roll;
    }

    internal void Deconstruct(out Range pitch, out Range yaw, out Range roll)
    {
      pitch = this.pitch;
      yaw = this.yaw;
      roll = this.roll;
    }
    public static EulerConstraint fullRotation = new EulerConstraint(new Range(360F), new Range(360F), new Range(360F));
    public static EulerConstraint noRotation = new EulerConstraint(new Range(0F), new Range(0F), new Range(0F));
  }

}




