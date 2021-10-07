using UnityEngine;

[DefaultExecutionOrder(-90)]
public class CustomHand : MonoBehaviour
{
  private HandTypes handType = HandTypes.HandLeft;
  [SerializeField]
  private Transform _pointerPoseRoot = null;
  private GameObject _pointerPoseGO;
  private OVRPlugin.HandState _handState = new OVRPlugin.HandState();

  public bool isDataValid;
  public bool isDataHighConfidence;
  public bool isTracked;
  public bool IsSystemGestureInProgress { get; private set; }
  public bool IsPointerPoseValid { get; private set; }
  public Transform PointerPose { get; private set; }
  public float HandScale { get; private set; }
  public TrackingConfidence HandConfidence { get; private set; }
  public bool IsDominantHand { get; private set; }

  public void Start()
  {
    handType = GetComponent<Handedness>().handType;
  }

  private void Awake()
  {
    _pointerPoseGO = new GameObject();
    PointerPose = _pointerPoseGO.transform;
    if (_pointerPoseRoot != null)
    {
      PointerPose.SetParent(_pointerPoseRoot, false);
    }

    GetHandState(OVRPlugin.Step.Render);
  }

  private void Update() => GetHandState(OVRPlugin.Step.Render);

  private void FixedUpdate()
  {
    if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR) return;

    GetHandState(OVRPlugin.Step.Physics);
  }

  private void GetHandState(OVRPlugin.Step step)
  {
    if (OVRPlugin.GetHandState(step, (OVRPlugin.Hand)handType, ref _handState))
    {
      isTracked = (_handState.Status & OVRPlugin.HandStatus.HandTracked) != 0;
      IsSystemGestureInProgress =
          (_handState.Status & OVRPlugin.HandStatus.SystemGestureInProgress) != 0;
      IsPointerPoseValid = (_handState.Status & OVRPlugin.HandStatus.InputStateValid) != 0;
      IsDominantHand = (_handState.Status & OVRPlugin.HandStatus.DominantHand) != 0;
      PointerPose.localPosition = _handState.PointerPose.Position.FromFlippedZVector3f();
      PointerPose.localRotation = _handState.PointerPose.Orientation.FromFlippedZQuatf();
      HandScale = _handState.HandScale;
      HandConfidence = (TrackingConfidence)_handState.HandConfidence;

      isDataValid = true;
      isDataHighConfidence = isTracked && HandConfidence == TrackingConfidence.High;
    }
    else
    {
      isTracked = false;
      IsSystemGestureInProgress = false;
      IsPointerPoseValid = false;
      PointerPose.localPosition = Vector3.zero;
      PointerPose.localRotation = Quaternion.identity;
      HandScale = 1.0f;
      HandConfidence = TrackingConfidence.Low;

      isDataValid = false;
      isDataHighConfidence = false;
    }
  }

  public bool GetFingerIsPinching(HandFinger finger) => isDataValid && (((int)_handState.Pinches & (1 << (int)finger)) != 0);

  public float GetFingerPinchStrength(HandFinger finger)
  {
    if (
        !isDataValid
        || _handState.PinchStrength == null
        || _handState.PinchStrength.Length != (int)OVRPlugin.HandFinger.Max
    ) return 0.0f;

    return _handState.PinchStrength[(int)finger];
  }

  public TrackingConfidence GetFingerConfidence(HandFinger finger)
  {
    if (
        !isDataValid
        || _handState.FingerConfidences == null
        || _handState.FingerConfidences.Length != (int)OVRPlugin.HandFinger.Max
    ) return TrackingConfidence.Low;

    return (TrackingConfidence)_handState.FingerConfidences[(int)finger];
  }

  public SkeletonPoseData GetSkeletonPoseData()
  {
    var data = new SkeletonPoseData();

    data.IsDataValid = isDataValid;
    if (!isDataValid) return data;

    data.RootPose = _handState.RootPose;
    data.RootScale = _handState.HandScale;
    data.BoneRotations = _handState.BoneRotations;
    data.IsDataHighConfidence = isTracked && HandConfidence == TrackingConfidence.High;

    return data;
  }

  public enum HandFinger
  {
    Thumb = OVRPlugin.HandFinger.Thumb,
    Index = OVRPlugin.HandFinger.Index,
    Middle = OVRPlugin.HandFinger.Middle,
    Ring = OVRPlugin.HandFinger.Ring,
    Pinky = OVRPlugin.HandFinger.Pinky,
  }

  public enum TrackingConfidence
  {
    Low = OVRPlugin.TrackingConfidence.Low,
    High = OVRPlugin.TrackingConfidence.High
  }


  public struct SkeletonPoseData
  {
    public OVRPlugin.Posef RootPose { get; set; }
    public float RootScale { get; set; }
    public OVRPlugin.Quatf[] BoneRotations { get; set; }
    public bool IsDataValid { get; set; }
    public bool IsDataHighConfidence { get; set; }
    public int SkeletonChangedCount { get; set; }
  }
}
