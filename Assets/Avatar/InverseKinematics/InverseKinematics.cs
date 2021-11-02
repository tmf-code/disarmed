using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Solve3D;

public class InverseKinematics : MonoBehaviour
{
  public float strength = 1.0F;
  public Transform wrist;
  public Transform shoulder;
  public Transform target;
  public Transform forearm;
  public Transform humerus;
  // public Transform forwardKinematicTarget;
  public float errorDistance;

  void Start()
  {
    // Attempt to automap bones
    var handedness = gameObject.GetComponentOrThrow<Handedness>();

    var hand = handedness.HandPrefix();

    wrist = wrist == null
      ? transform.FindChildRecursive($"b_{hand}_wrist")
      : wrist;

    forearm = forearm == null
      ? transform.FindChildRecursive($"b_{hand}_forearm_stub")
      : forearm;

    humerus = humerus == null
      ? transform.FindChildRecursive($"b_{hand}_humerus")
      : humerus;

    shoulder = shoulder == null
      ? transform.FindChildRecursive($"b_{hand}_shoulder")
      : shoulder;

    var targetName = handedness.IsLeft()
      ? "ShoulderLeft"
      : "ShoulderRight";

    target = target == null
      ? GameObject.Find(targetName).transform
      : target;
  }

  void Update()
  {
    errorDistance = ApplyIK(forearm, wrist, humerus, shoulder, target, strength);
  }

  public static float ApplyIK(Transform forearm, Transform wrist, Transform humerus, Transform shoulder, Transform target, float strength)
  {
    var baseTransform = new JointTransform(forearm.position, wrist.rotation);
    var bones = new List<Transform>() { forearm, humerus, shoulder };

    var links = bones.ToList()
      .GetRange(0, bones.Count - 1)
      .Select(
          (bone, index) =>
          {
            var nextBone = bones[index + 1];
            var rotation = bone.localRotation;
            var boneName = bone.name;

            var constraint = ArmConstraints.get(boneName);
            return new Link(rotation, constraint, nextBone.localPosition);
          }
      ).ToList();

    for (var index = 0; index < 50; index++)
    {
      var intermediateResults =
        Solve(
            links,
            baseTransform,
            target.position,
            SolveOptions.defaultOptions
        ).links;

      for (int resultIndex = 0; resultIndex < intermediateResults.Count; resultIndex++)
      {
        links[resultIndex] = intermediateResults[resultIndex];
      }
    }

    var (results, getErrorDistance, isWithinAcceptedError) = Solve(
        links,
        baseTransform,
        target.position,
        SolveOptions.defaultOptions
    );

    for (int resultIndex = 0; resultIndex < results.Count; resultIndex++)
    {
      var bone = bones[resultIndex];
      var link = results[resultIndex];
      bone.localRotation = Quaternion.Slerp(bone.localRotation, link.rotation, strength);
    }

    return getErrorDistance();
  }

  class ArmConstraints
  {
    //Z axis is swapped with x for all (ie x axis points forward) (pitch with roll)
    static readonly EulerConstraint b_l_forearm_stub = new EulerConstraint(
      pitch: new Range(20F),
      yaw: new Range(30F),
      roll: new Range(170F)
    );
    static readonly EulerConstraint b_r_forearm_stub = new EulerConstraint(
      pitch: new Range(20F),
      yaw: new Range(30F),
      roll: new Range(170F)
    );
    static readonly EulerConstraint b_l_humerus = new EulerConstraint(
      pitch: new Range(30F),
      yaw: new Range(min: 0F, max: 100F),
      roll: new Range(min: 0F, max: 140F)
    );
    static readonly EulerConstraint b_r_humerus = new EulerConstraint(
      pitch: new Range(30F),
      yaw: new Range(min: 0F, max: 100F),
      roll: new Range(min: 0F, max: 140F)
    );

    // Not currently used - Would be if we retarget the IK towards the chest. Have had mixed results.
    static readonly EulerConstraint b_l_shoulder = new EulerConstraint(
      pitch: new Range(min: 0F, max: 30F),
      yaw: new Range(min: 20F, max: 80F),
      roll: new Range(min: 0F, max: 100F)
    );
    static readonly EulerConstraint b_r_shoulder = new EulerConstraint(
      pitch: new Range(min: -30F, max: 0F),
      yaw: new Range(min: -80F, max: 20F),
      roll: new Range(min: 0F, max: 100F)
    );

    public static EulerConstraint get(string boneName) => boneName switch
    {
      "b_l_forearm_stub" => b_l_forearm_stub,
      "b_r_forearm_stub" => b_r_forearm_stub,
      "b_l_humerus" => b_l_humerus,
      "b_r_humerus" => b_r_humerus,
      "b_l_shoulder" => b_l_shoulder,
      "b_r_shoulder" => b_r_shoulder,
      _ => throw new System.Exception($"Could not get bone constraints for bone {boneName}"),
    };
  }
}

