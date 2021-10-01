using System;
using System.Linq;
using UnityEngine;

public static class QuaternionExtensions
{
  public static Quaternion Clamp(
      this Quaternion quaternion,
      Vector3 lowerBound,
      Vector3 upperBound
  )
  {
    var rotationAxis = new float[] { quaternion.x, quaternion.y, quaternion.z };
    var w = quaternion.w;

    var resultComponents = rotationAxis.Select(
            (component, index) =>
            {
              var angle = 2 * Mathf.Atan(component / w);
              var lower = lowerBound[index];
              var upper = upperBound[index];
              if (lower > upper)
                throw new Exception(
                        $"Lower bound should be less than upper bound for component {index}. Lower: {lower}, upper: {upper}"
                    );
              var clampedAngle = Mathf.Clamp(angle, lower, upper);
              return Mathf.Tan(0.5F * clampedAngle);
            }
        )
        .ToArray();

    var result = new Quaternion(
        resultComponents[0],
        resultComponents[1],
        resultComponents[2],
        1
    );
    return result.normalized;
  }
}





