using UnityEngine;

public static class QuaternionExtensions
{
  public static Quaternion Clamp(
      this Quaternion quaternion,
      Vector3 lowerBound,
      Vector3 upperBound
  )
  {
    var w = 1F / quaternion.w;

    var x = 2F * Mathf.Atan(w * quaternion.x);
    var y = 2F * Mathf.Atan(w * quaternion.y);
    var z = 2F * Mathf.Atan(w * quaternion.z);

    var clampedX = Mathf.Tan(0.5F * Mathf.Clamp(x, lowerBound.x * Mathf.Deg2Rad, upperBound.x * Mathf.Deg2Rad));
    var clampedY = Mathf.Tan(0.5F * Mathf.Clamp(y, lowerBound.y * Mathf.Deg2Rad, upperBound.y * Mathf.Deg2Rad));
    var clampedZ = Mathf.Tan(0.5F * Mathf.Clamp(z, lowerBound.z * Mathf.Deg2Rad, upperBound.z * Mathf.Deg2Rad));

    var result = new Quaternion(clampedX, clampedY, clampedZ, 1F).normalized;
    return result;
  }
}





