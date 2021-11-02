using System;

struct SolveOptions
{
  public static SolveOptions defaultOptions = new SolveOptions(
      0.1F,
      new Func<float, float>((float _) => 200.1F),
      0.1F
  );

  /**
 * Angle gap taken to calculate the gradient of the error function
 * Usually the default here will do.
 */

  public float deltaAngle;
  /**
 * Sets the 'speed' at which the algorithm converges on the target.
 * Larger values will cause oscillations, or vibrations about the target
 * Lower values may move too slowly. You should tune this manually
 *
 * Can either be a constant, or a function that returns a learning rate
  ((errorDistance: number) => number)
 */

  public Func<float, float> learningRate;
  /**
 * Useful if there is oscillations or vibration around the target
 * @default 0
 */
  public float acceptedError;

  public SolveOptions(float deltaAngle, Func<float, float> learningRate, float acceptedError)
  {
    this.deltaAngle = deltaAngle;
    this.learningRate = learningRate;
    this.acceptedError = acceptedError;
  }

  internal void Deconstruct(
      out float deltaAngle,
      out Func<float, float> learningRate,
      out float acceptedError
  )
  {
    deltaAngle = this.deltaAngle;
    learningRate = this.learningRate;
    acceptedError = this.acceptedError;
  }
}
