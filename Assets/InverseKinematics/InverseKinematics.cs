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

  public OVRHand.Hand handType = OVRHand.Hand.None;

  void Start()
  {
    // Attempt to automap bones
    var hand = handType == OVRHand.Hand.HandLeft ? "l" : "r";
    wrist = transform.FindChildRecursive($"b_{hand}_wrist");
    forearm = transform.FindChildRecursive($"b_{hand}_forearm_stub");
    humerus = transform.FindChildRecursive($"b_{hand}_humerus");
    shoulder = transform.FindChildRecursive($"b_{hand}_shoulder");

    Debug.LogWarning($"b_{hand}_wrist".ToString());
  }

  void Update()
  {
    var baseTransform = new Solve3D.JointTransform(forearm.position, wrist.rotation);
    Transform[] bones = new Transform[] { forearm, humerus, shoulder };

    Solve3D.Link[] links = bones.ToList().GetRange(0, bones.Length - 1).Select((bone, index) =>
   {
     var nextBone = bones[index + 1];
     var rotation = bone.localRotation;
     var boneName = bone.name;

     //  Solve3D.Con["constraints"] constraints:  =
     //     CONSTRAINTS[boneName][handedness];


     return new Solve3D.Link(rotation, null, nextBone.localPosition);
   }).ToArray();



    for (var index = 0; index < 10; index++)
    {
      var intermediateResults = Solve3D.Solve(links, baseTransform, target.position, SolveOptions.defaultFABRIKOptions).links;

      for (int resultIndex = 0; resultIndex < intermediateResults.Length; resultIndex++)
      {
        links[resultIndex] = intermediateResults[resultIndex];
      }
    }

    var (results, getErrorDistance, isWithinAcceptedError) = Solve3D.Solve(links, baseTransform, target.position, SolveOptions.defaultFABRIKOptions);

    errorDistance = getErrorDistance();
    var pos = Solve3D.GetEndEffectorPosition(results, baseTransform);
    // Debug.LogWarning(Vector3.Distance(pos, target.position));

    // forwardKinematicTarget.position = pos;


    for (int resultIndex = 0; resultIndex < results.Length; resultIndex++)
    {
      var bone = bones[resultIndex]!;
      var link = results[resultIndex];
      bone.localRotation = link.rotation;
    }
  }
}
