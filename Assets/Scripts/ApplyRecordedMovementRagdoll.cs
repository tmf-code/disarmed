using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyRecordedMovementRagdoll : MonoBehaviour
{
  private Rigidbody model;

  [ShowOnly] public int playbackFrameRate = 10;
  [ShowOnly] public int framesPlayed = 0;
  [ShowOnly] public float strength = 10F / 60F;

  private ObjectToFramesDictionary recording;

  private ChildDictionary childDictionary;
  private Dictionary<GameObject, UnSerializedTransform[]> recordingPairs;

  private Transform forearm;
  private Transform humerus;
  private Transform shoulder;
  private ConfigurableJoint forearmJoint;
  private ConfigurableJoint humerusJoint;

  void Start()
  {
    var hand = gameObject.GetComponentOrThrow<Handedness>().handType;
    recording = GameObject.Find("Recordings").GetComponentOrThrow<RecordingsStore>().RandomRecording(hand);
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    model = childDictionary.model.gameObject.GetComponentOrThrow<Rigidbody>();

    forearm = childDictionary.modelForearm;
    humerus = childDictionary.modelHumerus;
    shoulder = childDictionary.modelShoulder;

    LoadAndPlay();
  }

  void LoadAndPlay()
  {

    recordingPairs = RecordingToPairedTarget(
        recording,
        childDictionary.model.transform,
        childDictionary.modelChildren
    );

    CancelInvoke();
    InvokeRepeating(nameof(PlayNextFrame), 0f, 1F / playbackFrameRate);
  }

  private static Dictionary<GameObject, UnSerializedTransform[]> RecordingToPairedTarget(ObjectToFramesDictionary recording, Transform root, StringGameObjectDictionary targets)
  {
    Dictionary<GameObject, UnSerializedTransform[]> result = new Dictionary<GameObject, UnSerializedTransform[]>();

    foreach (var nameTransforms in recording)
    {
      var name = nameTransforms.Key;
      var transforms = nameTransforms.Value;
      if (name.Contains("handMeshNode"))
      {
        continue;
      }
      var target = name == "Model" ? root.gameObject : targets.GetValue(name).Unwrap();
      result.Add(target, transforms);
      continue;
    }

    return result;
  }

  void PlayNextFrame()
  {
    var numFrames = recording.First().Value.Length;
    framesPlayed += 1;
    framesPlayed %= numFrames;
  }

  void FixedUpdate()
  {

    foreach (var pair in recordingPairs)
    {
      var objectToChange = pair.Key;
      var transform = pair.Value[framesPlayed];

      if (objectToChange == model.gameObject)
      {
        // Make the base move
        var modelForceDirection = Quaternion.Inverse(model.transform.localRotation) * transform.localRotation;
        var modelRigidBody = model.GetComponent<Rigidbody>();
        modelRigidBody.AddRelativeTorque(new Vector3(modelForceDirection[0], modelForceDirection[1], modelForceDirection[2]) * 500F);
      }
      else if (objectToChange == forearm.gameObject)
      {
        forearmJoint = forearm.gameObject.GetComponentIfNull(forearmJoint);
        SetTargetRotationInternal(forearmJoint, transform.localRotation, forearm.localRotation, Space.Self);
      }
      else if (objectToChange == humerus.gameObject)
      {
        humerusJoint = humerus.gameObject.GetComponentIfNull(humerusJoint);
        SetTargetRotationInternal(humerusJoint, transform.localRotation, humerus.localRotation, Space.Self);
      }
      else
      {
        objectToChange.transform.localRotation = Quaternion.SlerpUnclamped(objectToChange.transform.localRotation, transform.localRotation, strength);
      }
    }
  }

  private static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
  {
    // Calculate the rotation expressed by the joint's axis and secondary axis
    var right = joint.axis;
    var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
    var up = Vector3.Cross(forward, right).normalized;
    Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

    // Transform into world space
    Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

    // Counter-rotate and apply the new local rotation.
    // Joint space is the inverse of world space, so we need to invert our value
    if (space == Space.World)
    {
      resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
    }
    else
    {
      resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
    }

    // Transform back into joint space
    resultRotation *= worldToJointSpace;

    // Set target rotation to our newly calculated rotation
    joint.targetRotation = resultRotation;
  }
}
