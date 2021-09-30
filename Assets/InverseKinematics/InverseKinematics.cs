using System.Linq;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
  public Transform wrist;
  public Transform shoulder;
  public Transform target;
  public Transform forearm;
  public Transform humerus;

  public Transform absForearm;
  public Transform absHumerus;

  public float errorDistance;

  public Transform forwardKinematicTarget;

  void Update()
  {

    var baseTransform = new Solve3D.JointTransform(forearm.position, wrist.rotation);
    Transform[] bones = new Transform[] { forearm, humerus, shoulder };

    Solve3D.Link[] links = bones.ToList().GetRange(0, bones.Length - 1).Select((bone, index) =>
   {
     var nextBone = bones[index + 1]!;
     var rotation = bone.localRotation;
     var boneName = bone.name;

     //  Solve3D.Con["constraints"] constraints:  =
     //     CONSTRAINTS[boneName][handedness];


     return new Solve3D.Link(rotation, null, nextBone.localPosition);
   }).ToArray();



    for (var index = 0; index < 100; index++)
    {
      var intermediateResults = Solve3D.Solve(links, baseTransform, target.position, SolveOptions.defaultFABRIKOptions).links;

      for (int resultIndex = 0; resultIndex < intermediateResults.Length; resultIndex++)
      {
        links[resultIndex] = intermediateResults[resultIndex];
      }
    }

    var (results, getErrorDistance, isWithinAcceptedError) = Solve3D.Solve(links, baseTransform, target.position, SolveOptions.defaultFABRIKOptions);

    errorDistance = getErrorDistance();
    forwardKinematicTarget.position = Solve3D.GetEndEffectorPosition(results, baseTransform);

    Transform[] realBones = new Transform[] { absForearm, absHumerus };


    for (int resultIndex = 0; resultIndex < results.Length; resultIndex++)
    {
      var bone = bones[resultIndex]!;
      var link = results[resultIndex];
      bone.localRotation = link.rotation;

      realBones[resultIndex].position = link.position;
    }
  }
}
