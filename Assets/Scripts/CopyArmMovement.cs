using UnityEngine;

public class CopyArmMovement : MonoBehaviour
{
  [Range(0, 1)]
  public float strength = 10F / 60F;
  public GameObject targetArm;
  private ChildDictionary targetChildDictionary;
  [SerializeField]
  [HideInInspector]
  private TransformPair[] handBonePairs;
  private ChildDictionary childDictionary;
  private Handedness handedness;
  private Transform forearm;
  private Transform humerus;
  private Transform forearmOther;
  private Transform humerusOther;

  void Start()
  {
    childDictionary = gameObject.GetComponentOrThrow<ChildDictionary>();
    handedness = gameObject.GetComponentOrThrow<Handedness>();

    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    targetArm = handedness.handType switch
    {
      Handedness.HandTypes.HandLeft => playerArms.left,
      _ => playerArms.right,
    };

    targetChildDictionary = targetArm.GetComponentOrThrow<ChildDictionary>();

    handBonePairs = targetChildDictionary.handBonePairs;

    var handPrefix = handedness.HandPrefix();

    forearm = childDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerus = childDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

    forearmOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_forearm_stub").Unwrap().transform;
    humerusOther = targetChildDictionary.modelChildren.GetValue($"b_{handPrefix}_humerus").Unwrap().transform;

  }

  void Update()
  {
    // Copy IK from other arm
    forearm.localRotation = Quaternion.SlerpUnclamped(forearmOther.localRotation, forearm.localRotation, strength);
    humerus.localRotation = Quaternion.SlerpUnclamped(humerusOther.localRotation, humerus.localRotation, strength);

    foreach (var sourceAndDestination in handBonePairs)
    {
      var source = sourceAndDestination.Item1;
      var current = sourceAndDestination.Item2;
      current.LerpLocal(source, strength);
    }
  }
}
