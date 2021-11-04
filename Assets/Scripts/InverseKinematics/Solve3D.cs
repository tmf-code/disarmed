using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

partial class Solve3D
{
  /**
 * Changes joint angle to minimize distance of end effector to target
 */
  public static SolveResult Solve(
      List<Link> links,
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
          links.ConvertAll(Link.CopyLink),
          () => error,
          true
      );

    if (joints.Count != links.Count + 1)
    {
      throw new Exception(
          $"Joint transforms should have the same length as links + 1.Got {joints.Count}, expected {links.Count}"
      );
    }

    var withAngleStep = links.Select((link, linkIndex) =>
    {
      var position = link.position;
      var rotation = link.rotation;
      var constraints = link.constraints;

      // For each, calculate partial derivative, sum to give full numerical derivative
      var vectorComponents = new List<float>() { 0, 0, 0 }.Select(
        (_, v3Index) =>
        {
          var eulerAngle = new List<float>() { 0, 0, 0 };
          eulerAngle[v3Index] = deltaAngle;
          var angleDelta =
            rotation
            * Quaternion.Euler(eulerAngle[0], eulerAngle[1], eulerAngle[2]);
          var linkWithAngleDelta = new Link(angleDelta, link.constraints, position);

          // Get remaining links from this links joint
          var projectedLinks = new List<Link>() { linkWithAngleDelta }.Concat(
            links.ToList().Skip(linkIndex + 1)
          ).ToList();

          var joint = joints[linkIndex];
          var projectedError = GetErrorDistance(
            projectedLinks,
            joint,
            target
          );
          var gradient = projectedError / deltaAngle - error / deltaAngle;

          var angleStep = -gradient * learningRate(projectedError);

          return angleStep;
        }).ToArray();

      var steppedRotation =
        rotation
        * Quaternion.Euler(
            vectorComponents[0],
            vectorComponents[1],
            vectorComponents[2]
        );

      return new Link(steppedRotation, constraints, position);
    }).ToList();

    var withConstraints = ApplyConstraints(withAngleStep);

    return new SolveResult(
        withConstraints,
        () => GetErrorDistance(withAngleStep, baseJoint, target),
        null
    );
  }
  /**
 * Returns the absolute position and rotation of each link
 */
  public static JointTransformResult GetJointTransforms(List<Link> links, JointTransform joint)
  {
    JointTransform jointCopy = joint;
    var transforms = new List<JointTransform>() { jointCopy };

    for (var index = 0; index < links.Count; index++)
    {
      var currentLink = links[index]!;
      var parentTransform = transforms[index]!;

      var absoluteRotation = parentTransform.rotation * currentLink.rotation;

      var relativePosition = absoluteRotation * currentLink.position;
      var absolutePosition = relativePosition + parentTransform.position;
      transforms.Add(new JointTransform(absolutePosition, absoluteRotation));
    }

    var effectorPosition = transforms.Last().position;

    return new JointTransformResult(transforms, effectorPosition);
  }

  static Link ApplyConstraint(Link link)
  {
    var (rotation, constraints, position) = link;

    var (pitch, yaw, roll) = constraints;
    var lowerBound = new Vector3(pitch.min, yaw.min, roll.min);
    var upperBound = new Vector3(pitch.max, yaw.max, roll.max);
    var clampedRotation = QuaternionExtensions.Clamp(rotation, lowerBound, upperBound);
    return new Link(clampedRotation, CopyConstraints(constraints), position);
  }


  static List<Link> ApplyConstraints(List<Link> links) => links.ConvertAll(ApplyConstraint);

  /**
  * Distance from end effector to the target
*/
  static float GetErrorDistance(List<Link> links, JointTransform baseTransform, Vector3 target)
  {
    var effectorPosition = GetEndEffectorPosition(links, baseTransform);
    return Vector3.Distance(target, effectorPosition);
  }

  /**
  * Absolute position of the end effector (last links tip)
*/
  public static Vector3 GetEndEffectorPosition(List<Link> links, JointTransform joint)
  {
    return GetJointTransforms(links, joint).effectorPosition;
  }

  public struct JointTransformResult
  {
    public readonly List<JointTransform> transforms;
    public readonly Vector3 effectorPosition;

    public JointTransformResult(List<JointTransform> transforms, Vector3 effectorPosition)
    {
      this.transforms = transforms;
      this.effectorPosition = effectorPosition;
    }
  }
}




