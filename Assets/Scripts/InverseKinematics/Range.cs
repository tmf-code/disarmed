public struct Range
{
  public readonly float min;
  public readonly float max;

  public Range(float min, float max)
  {
    this.min = min;
    this.max = max;
  }

  public Range(float amount)
  {
    min = -amount / 2;
    max = amount / 2;
  }
}
