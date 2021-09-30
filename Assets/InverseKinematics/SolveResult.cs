using System;

partial class Solve3D
{
  public struct SolveResult
  {
    /**
     * Copy of the structure of input links
     * With the possibility of their rotation being changed
     */
    public readonly Link[] links;
    /**
     * Returns the error distance after the solve step
     */
    public readonly Func<float> getErrorDistance;
    /**
     * true if the solve terminates early due to the end effector being close to the target.
     * null if solve has adjusted the rotations in links
     *
     * null is used here as we don't rerun error checking after the angle adjustment, thus it cannot be known true or false.
     * This is done to improve performance
     */
    public readonly bool? isWithinAcceptedError;

    public SolveResult(Link[] links, Func<float> getErrorDistance, bool? isWithinAcceptedError)
    {
      this.links = links;
      this.getErrorDistance = getErrorDistance;
      this.isWithinAcceptedError = isWithinAcceptedError;
    }

    internal void Deconstruct(out Link[] links, out Func<float> getErrorDistance, out bool? isWithinAcceptedError)
    {
      links = this.links;
      getErrorDistance = this.getErrorDistance;
      isWithinAcceptedError = this.isWithinAcceptedError;
    }
  }
}


