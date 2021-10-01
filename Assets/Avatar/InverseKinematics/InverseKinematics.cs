using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
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
    var handType = gameObject.GetComponent<Handedness>().handType;

    var hand = handType == CustomHand.HandTypes.HandLeft ? "l" : "r";

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

    var targetName = handType == CustomHand.HandTypes.HandLeft
      ? "ShoulderLeft"
      : "ShoulderRight";

    target = target == null
      ? GameObject.Find(targetName).transform
      : target;
  }

  void Update()
  {
    var baseTransform = new Solve3D.JointTransform(forearm.position, wrist.rotation);
    Transform[] bones = new Transform[] { forearm, humerus, shoulder };

    Solve3D.Link[] links = bones.ToList()
        .GetRange(0, bones.Length - 1)
        .Select(
            (bone, index) =>
            {
              var nextBone = bones[index + 1];
              var rotation = bone.localRotation;
              var boneName = bone.name;

              //  Solve3D.Con["constraints"] constraints:  =
              //     CONSTRAINTS[boneName][handedness];


              return new Solve3D.Link(rotation, null, nextBone.localPosition);
            }
        )
        .ToArray();

    for (var index = 0; index < 10; index++)
    {
      var intermediateResults =
          Solve3D.Solve(
              links,
              baseTransform,
              target.position,
              SolveOptions.defaultFABRIKOptions
          ).links;

      for (int resultIndex = 0; resultIndex < intermediateResults.Length; resultIndex++)
      {
        links[resultIndex] = intermediateResults[resultIndex];
      }
    }

    var (results, getErrorDistance, isWithinAcceptedError) = Solve3D.Solve(
        links,
        baseTransform,
        target.position,
        SolveOptions.defaultFABRIKOptions
    );

    errorDistance = getErrorDistance();
    var pos = Solve3D.GetEndEffectorPosition(results, baseTransform);

    for (int resultIndex = 0; resultIndex < results.Length; resultIndex++)
    {
      var bone = bones[resultIndex]!;
      var link = results[resultIndex];
      bone.localRotation = link.rotation;
    }
  }
}
