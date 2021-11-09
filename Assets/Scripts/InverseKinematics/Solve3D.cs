using System;
using System.Linq;
using UnityEngine;

partial class Solve3D
{
  /**
 * Changes joint angle to minimize distance of end effector to target
 */
  public static SolveResult Solve(
      Link[] links,
      JointTransform baseJoint,
      Vector3 target,
      SolveOptions options
  )
  {
    var (deltaAngle, learningRate, acceptedError) = options;
    var transforms = GetJointTransforms(links, baseJoint);
    var joints = transforms.transforms;

    var effectorPosition = transforms.effectorPosition;
    var error = Vector3.Distance(target, effectorPosition);

    if (error < acceptedError)
      return new SolveResult(
          links,
          () => error,
          true
      );

    if (joints.Length != links.Length + 1)
    {
      throw new Exception(
          $"Joint transforms should have the same length as links + 1.Got {joints.Length}, expected {links.Length}"
      );
    }

    var withAngleStep = new Link[links.Length];

    var deltaAngles = new Quaternion[3] {
      Quaternion.Euler( deltaAngle, 0, 0 ),
      Quaternion.Euler(0, deltaAngle, 0 ),
      Quaternion.Euler(0, 0, deltaAngle),
    };

    for (int linkIndex = 0; linkIndex < links.Length; linkIndex++)
    {
      var link = links[linkIndex];
      var position = link.position;
      var rotation = link.rotation;
      var constraints = link.constraints;

      var remainingLinks = links.ToList().Skip(linkIndex + 1).ToArray();

      // For each, calculate partial derivative, sum to give full numerical derivative

      var vectorComponents = new float[3];
      for (int v3Index = 0; v3Index < 3; v3Index++)
      {
        var eulerAngle = deltaAngles[v3Index];
        var angleDelta = rotation * eulerAngle;
        var linkWithAngleDelta = new Link(angleDelta, link.constraints, position);

        // Get remaining links from this links joint
        var projectedLinks = new Link[1 + remainingLinks.Length];
        projectedLinks[0] = linkWithAngleDelta;
        Array.Copy(remainingLinks, 0, projectedLinks, 1, remainingLinks.Length);

        var joint = joints[linkIndex];
        var projectedError = GetErrorDistance(
          projectedLinks,
          joint,
          target
        );
        var gradient = projectedError / deltaAngle - error / deltaAngle;

        var angleStep = -gradient * learningRate(projectedError);

        vectorComponents[v3Index] = angleStep;
      }

      var steppedRotation =
        rotation
        * Quaternion.Euler(
            vectorComponents[0],
            vectorComponents[1],
            vectorComponents[2]
        );

      withAngleStep[linkIndex] = new Link(steppedRotation, constraints, position);
    }

    var withConstraints = ApplyConstraints(withAngleStep);

    return new SolveResult(
        withConstraints,
        () => GetErrorDistance(withConstraints, baseJoint, target),
        null
    );
  }
  /**
 * Returns the absolute position and rotation of each link
 */
  public static JointTransformResult GetJointTransforms(Link[] links, JointTransform joint)
  {
    var transforms = new JointTransform[links.Length + 1];
    transforms[0] = joint;

    for (var index = 0; index < links.Length; index++)
    {
      var currentLink = links[index];
      var parentTransform = transforms[index];

      var absoluteRotation = parentTransform.rotation * currentLink.rotation;

      var relativePosition = absoluteRotation * currentLink.position;
      var absolutePosition = relativePosition + parentTransform.position;
      transforms[index + 1] = new JointTransform(absolutePosition, absoluteRotation);
    }

    var effectorPosition = transforms[transforms.Length - 1].position;

    return new JointTransformResult(transforms, effectorPosition);
  }

  static Link[] ApplyConstraints(Link[] links)
  {
    for (int linkIndex = 0; linkIndex < links.Length; linkIndex++)
    {
      Link.ApplyConstraint(ref links[linkIndex]);
    }

    return links;
  }

  /**
  * Distance from end effector to the target
*/
  static float GetErrorDistance(Link[] links, JointTransform baseTransform, Vector3 target)
  {
    var effectorPosition = GetEndEffectorPosition(links, baseTransform);
    return Vector3.Distance(target, effectorPosition);
  }

  /**
  * Absolute position of the end effector (last links tip)
*/
  public static Vector3 GetEndEffectorPosition(Link[] links, JointTransform joint)
  {
    return GetJointTransforms(links, joint).effectorPosition;
  }

  public struct JointTransformResult
  {
    public readonly JointTransform[] transforms;
    public readonly Vector3 effectorPosition;

    public JointTransformResult(JointTransform[] transforms, Vector3 effectorPosition)
    {
      this.transforms = transforms;
      this.effectorPosition = effectorPosition;
    }
  }
}




