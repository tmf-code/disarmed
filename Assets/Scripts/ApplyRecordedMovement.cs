using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ApplyRecordedMovement : MonoBehaviour
{
  [Range(0, 1)]
  private Transform forearmIK;
  private Transform humerusIK;

  private Transform forearm;
  private Transform humerus;

  private Transform trackingData;
  private Rigidbody model;
  private Tuple<Transform, Transform>[] handBonePairs;

  [ShowOnly] public int playbackFrameRate = 10;
  [ShowOnly] public int framesPlayed = 0;

  public float strength = 10F / 60F;

  [ShowOnly] public readonly string recordingsPath = "Recordings";
  public RecordedMovementTypes recordingName = RecordedMovementTypes.Unset;

  private Option<ArmRecording> recording;
  private ChildDictionary childDictionary;
  private Option<Dictionary<GameObject, ITransform[]>> recordingPairs;

  public enum RecordedMovementTypes
  {
    Unset,
    LeftAction,
    RightAction,
  }

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    var hand = gameObject.GetComponentOrThrow<Handedness>().HandPrefix();

    forearmIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerusIK = childDictionary.vrTrackingDataChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;

    forearm = childDictionary.modelChildren.GetValue($"b_{hand}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{hand}_humerus").Unwrap().transform;

    trackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    model = transform.FindRecursiveOrThrow("Model").gameObject.GetComponentOrThrow<Rigidbody>();

    handBonePairs = childDictionary.vrTrackingDataChildren.Values.Where(child =>
    {
      return BoneNameOperations.IsTrackedBone(child.name) && child.name != "b_l_forearm_stub" && child.name != "b_r_forearm_stub";
    }).Select(bone => new Tuple<Transform, Transform>(bone.transform, childDictionary.modelChildren.GetValue(bone.name).Unwrap().transform)).ToArray();

    LoadAndPlayRandom();
  }

  void LoadAndPlayRandom()
  {
    if (gameObject.GetComponentOrThrow<Handedness>().IsLeft())
    {
      var recordingName = Enum.GetNames(typeof(RecordedMovementTypes)).Where(name => name.Contains("Left")).First();
      this.recordingName = recordingName.ConvertToEnum<RecordedMovementTypes>();
      LoadAndPlay(this.recordingName);
    }
    else
    {
      var recordingName = Enum.GetNames(typeof(RecordedMovementTypes)).Where(name => name.Contains("Right")).First();
      this.recordingName = recordingName.ConvertToEnum<RecordedMovementTypes>();
      LoadAndPlay(this.recordingName);
    }
  }

  void LoadAndPlay(RecordedMovementTypes recordingName)
  {
    if (recordingName == RecordedMovementTypes.Unset)
    {
      recording = new None<ArmRecording>();
      recordingPairs = new None<Dictionary<GameObject, ITransform[]>>();
    }
    else
    {
      var hand = gameObject.GetComponentOrThrow<Handedness>().handType;
      recording = Option<ArmRecording>.of(LoadRecording(recordingsPath, recordingName));
      recordingPairs = recording.Map(recording => RecordingToPairedTarget(
          recording,
          childDictionary.vrTrackingData.transform,
          childDictionary.vrTrackingDataChildren,
          hand)
        );
    }

    if (recordingPairs.IsSome() && recording.IsSome())
    {
      InvokeRepeating(nameof(PlayNextFrame), 0f, 1F / playbackFrameRate);
    }
    else
    {
      CancelInvoke();
    }
  }

  static Dictionary<GameObject, ITransform[]> RecordingToPairedTarget(ArmRecording recording, Transform vrTrackingData, StringGameObjectDictionary targets, Handedness.HandTypes hand)
  {
    if (recording.hand != hand) throw new Exception("Cannot create recording for opposite hand");

    Dictionary<GameObject, ITransform[]> result = new Dictionary<GameObject, ITransform[]>
    {
      { vrTrackingData.gameObject, new ITransform[recording.frameTransforms.Count] }
    };

    foreach (var nameObject in targets)
    {
      var gameObject = nameObject.Value;
      result.Add(gameObject, new ITransform[recording.frameTransforms.Count]);
    }

    for (int frameIndex = 0; frameIndex < recording.frameTransforms.Count; frameIndex++)
    {
      var frame = recording.frameTransforms[frameIndex];

      foreach (var nameTransform in frame)
      {
        var name = nameTransform.Key;

        // Don't care about mesh location as it does nothing
        if (name.Contains("handMeshNode"))
        {
          continue;
        }

        // Recordings are taken on Model, but need to be applied to VRTrackingData
        if (name == "Model")
        {
          var transform = nameTransform.Value;
          var target = vrTrackingData.gameObject;
          result.GetValue(target).Unwrap()[frameIndex] = transform;

        }
        else
        {
          targets.GetValue(name).Match(none: () =>
          {
            Debug.Log($"Could not get value for key {name}");
          }, some: (gameObject) =>
         {
           var transform = nameTransform.Value;
           result.GetValue(gameObject).Unwrap()[frameIndex] = transform;
         });
        }
      }
    }

    return result;
  }

  void PlayNextFrame()
  {
    framesPlayed += 1;
    framesPlayed %= recording.Unwrap().frameTransforms.Count;
  }

  public static ArmRecording LoadRecording(string recordingsPath, RecordedMovementTypes recordingName)
  {
#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
    var recordingPath = $"{recordingsPath}/{recordingName}";
    var textAsset = Resources.Load<TextAsset>(recordingPath);
    if (textAsset == null)
    {
      throw new Exception($"Could not load text asset {recordingPath}");
    }
    return JsonUtility.FromJson<ArmRecording>(textAsset.text);
  }

  void Update()
  {
    // Apply the recording to the VR tracking object
    foreach (var pair in recordingPairs.Unwrap())
    {
      var gameObject = pair.Key;
      var transform = pair.Value;

      gameObject.transform.LerpLocal(transform[framesPlayed], strength);
    }
  }

  void FixedUpdate()
  {
    // Make the IK joints move
    var forearmJoint = forearm.GetComponent<ConfigurableJoint>();
    SetTargetRotationInternal(forearmJoint, forearmIK.localRotation, forearm.localRotation, Space.Self);

    var humerusJoint = humerus.GetComponent<ConfigurableJoint>();
    SetTargetRotationInternal(humerusJoint, humerusIK.localRotation, humerus.localRotation, Space.Self);


    // Make the base move
    var modelForceDirection = Quaternion.Inverse(model.transform.localRotation) * trackingData.localRotation;
    var modelRigidBody = model.GetComponent<Rigidbody>();
    modelRigidBody.AddRelativeTorque(new Vector3(modelForceDirection[0], modelForceDirection[1], modelForceDirection[2]) * 500F);

    // Make the fingers move
    foreach (var pair in handBonePairs)
    {
      var current = pair.Item2;
      current.LerpLocal(pair.Item1, strength);
    }
  }

  static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
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
