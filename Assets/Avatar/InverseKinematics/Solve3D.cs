using System;
using System.Linq;
using UnityEngine;

partial class Solve3D
{
  /**
 * Changes joint angle to minimize distance of end effector to target
 *
 * If given no options, runs in FABRIK mode
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
          links.Select((link) => Link.CopyLink(link)).ToArray(),
          () => error,
          true
      );

    if (joints.Length != links.Length + 1)
    {
      throw new Exception(
          $"Joint transforms should have the same length as links + 1.Got {joints.Length}, expected {links.Length}"
      );
    }

    Link[] withAngleStep = links.Select(
            (link, linkIndex) =>
            {
              var position = link.position;
              var rotation = link.rotation;
              var constraints = link.constraints;

                  // For each, calculate partial derivative, sum to give full numerical derivative
                  var vectorComponents = new float[] { 0, 0, 0 }.Select(
                          (_, v3Index) =>
                          {
                          var eulerAngle = new float[] { 0, 0, 0 };
                          eulerAngle[v3Index] = deltaAngle;
                          var angleDelta =
                                  rotation
                                  * Quaternion.Euler(eulerAngle[0], eulerAngle[1], eulerAngle[2]);
                          var linkWithAngleDelta = new Link(angleDelta, null, position);

                              // Get remaining links from this links joint
                              Link[] projectedLinks = new Link[] { linkWithAngleDelta }.Concat(
                                      links.ToList().Skip(linkIndex + 1)
                                  )
                                  .ToArray();

                          var joint = joints[linkIndex];
                          var projectedError = GetErrorDistance(
                                  projectedLinks,
                                  joint,
                                  target
                              );
                          var gradient = projectedError / deltaAngle - error / deltaAngle;

                          var angleStep = -gradient * learningRate(projectedError);

                          return angleStep;
                        }
                      )
                      .ToArray();

              var steppedRotation =
                      rotation
                      * Quaternion.Euler(
                          vectorComponents[0],
                          vectorComponents[1],
                          vectorComponents[2]
                      );

              return new Link(steppedRotation, constraints, position);
            }
        )
        .ToArray();

    var adjustedJoints = GetJointTransforms(withAngleStep, baseJoint).transforms;
    var withConstraints = ApplyConstraints(withAngleStep, adjustedJoints);

    return new SolveResult(
        withAngleStep,
        () => GetErrorDistance(withAngleStep, baseJoint, target),
        null
    );
  }
  /**
 * Returns the absolute position and rotation of each link
 */
  public static JointTransformResult GetJointTransforms(Link[] links, JointTransform joint)
  {
    JointTransform jointCopy = joint;
    var transforms = new JointTransform[] { jointCopy }.ToList();

    for (var index = 0; index < links.Length; index++)
    {
      var currentLink = links[index]!;
      var parentTransform = transforms[index]!;

      var absoluteRotation = parentTransform.rotation * currentLink.rotation;

      var relativePosition = absoluteRotation * currentLink.position;
      var absolutePosition = relativePosition + parentTransform.position;
      transforms.Add(new JointTransform(absolutePosition, absoluteRotation));
    }

    var effectorPosition = transforms.Last().position;

    return new JointTransformResult(transforms.ToArray(), effectorPosition);
  }

  static Link ApplyConstraint(Link link, JointTransform joint)
  {
    var (rotation, constraints, position) = link;
    if (constraints == null)
      return new Link(rotation, null, position);

    return constraints.Match(
        Right: new Func<ExactRotation, Link>(
            constraint =>
            {
              if (constraint.type == ExactRotation.ExactRotationType.GLOBAL)
              {
                var targetRotation = constraint.value;
                var currentRotation = joint.rotation;
                var adjustedRotation =
                        rotation * Quaternion.Inverse(currentRotation) * targetRotation;

                return new Link(adjustedRotation, constraints, position);
              }
              else
              {
                return new Link(constraint.value, constraints, position);
              }
            }
        ),
        Left: constraint =>
        {
          var (pitch, yaw, roll) = constraint;

          float pitchMin;
          float pitchMax;
          if (pitch != null)
          {
            pitchMin = pitch.Value.min;
            pitchMax = pitch.Value.max;
          }
          else
          {
            pitchMin = float.NegativeInfinity;
            pitchMax = float.PositiveInfinity;
          }

          float yawMin;
          float yawMax;
          if (yaw != null)
          {
            yawMin = yaw.Value.min;
            yawMax = yaw.Value.max;
          }
          else
          {
            yawMin = float.NegativeInfinity;
            yawMax = float.PositiveInfinity;
          }

          float rollMin;
          float rollMax;
          if (roll != null)
          {
            rollMin = roll.Value.min;
            rollMax = roll.Value.max;
          }
          else
          {
            rollMin = float.NegativeInfinity;
            rollMax = float.PositiveInfinity;
          }

          var lowerBound = new Vector3(pitchMin, yawMin, rollMin);
          var upperBound = new Vector3(pitchMax, yawMax, rollMax);
          var clampedRotation = QuaternionExtensions.Clamp(rotation, lowerBound, upperBound);
          return new Link(rotation, CopyConstraints(constraints), position);
        }
    );
  }

  static Link[] ApplyConstraints(Link[] links, JointTransform[] joints)
  {
    return links.Select((link, index) => ApplyConstraint(link, joints[index + 1]!)).ToArray();
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




