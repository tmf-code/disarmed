using UnityEngine;

[DefaultExecutionOrder(-90)]
public class CustomHand
    : MonoBehaviour,
      Skeleton.ISkeletonDataProvider,
      OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider,
      OVRMesh.IOVRMeshDataProvider,
      OVRMeshRenderer.IOVRMeshRendererDataProvider
{
  public enum HandTypes
  {
    None = OVRPlugin.Hand.None,
    HandLeft = OVRPlugin.Hand.HandLeft,
    HandRight = OVRPlugin.Hand.HandRight,
  }

  public enum HandFinger
  {
    Thumb = OVRPlugin.HandFinger.Thumb,
    Index = OVRPlugin.HandFinger.Index,
    Middle = OVRPlugin.HandFinger.Middle,
    Ring = OVRPlugin.HandFinger.Ring,
    Pinky = OVRPlugin.HandFinger.Pinky,
    Max = OVRPlugin.HandFinger.Max,
  }

  public enum TrackingConfidence
  {
    Low = OVRPlugin.TrackingConfidence.Low,
    High = OVRPlugin.TrackingConfidence.High
  }

  private HandTypes HandType = HandTypes.None;
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
    HandType = GetComponent<Handedness>().handType;
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

  private void Update()
  {
    GetHandState(OVRPlugin.Step.Render);
  }

  private void FixedUpdate()
  {
    if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
    {
      GetHandState(OVRPlugin.Step.Physics);
    }
  }

  private void GetHandState(OVRPlugin.Step step)
  {
    if (OVRPlugin.GetHandState(step, (OVRPlugin.Hand)HandType, ref _handState))
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

  public bool GetFingerIsPinching(HandFinger finger)
  {
    return isDataValid && (((int)_handState.Pinches & (1 << (int)finger)) != 0);
  }

  public float GetFingerPinchStrength(HandFinger finger)
  {
    if (
        isDataValid
        && _handState.PinchStrength != null
        && _handState.PinchStrength.Length == (int)OVRPlugin.HandFinger.Max
    )
    {
      return _handState.PinchStrength[(int)finger];
    }

    return 0.0f;
  }

  public TrackingConfidence GetFingerConfidence(HandFinger finger)
  {
    if (
        isDataValid
        && _handState.FingerConfidences != null
        && _handState.FingerConfidences.Length == (int)OVRPlugin.HandFinger.Max
    )
    {
      return (TrackingConfidence)_handState.FingerConfidences[(int)finger];
    }

    return TrackingConfidence.Low;
  }

  Skeleton.SkeletonType Skeleton.ISkeletonDataProvider.GetSkeletonType()
  {
    return HandType switch
    {
      HandTypes.HandLeft => Skeleton.SkeletonType.HandLeft,
      HandTypes.HandRight => Skeleton.SkeletonType.HandRight,
      _ => Skeleton.SkeletonType.None,
    };
  }

  Skeleton.SkeletonPoseData Skeleton.ISkeletonDataProvider.GetSkeletonPoseData()
  {
    var data = new Skeleton.SkeletonPoseData();

    data.IsDataValid = isDataValid;
    if (isDataValid)
    {
      data.RootPose = _handState.RootPose;
      data.RootScale = _handState.HandScale;
      data.BoneRotations = _handState.BoneRotations;
      data.IsDataHighConfidence = isTracked && HandConfidence == TrackingConfidence.High;
    }

    return data;
  }

  OVRSkeletonRenderer.SkeletonRendererData OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider.GetSkeletonRendererData()
  {
    var data = new OVRSkeletonRenderer.SkeletonRendererData();

    data.IsDataValid = isDataValid;
    if (isDataValid)
    {
      data.RootScale = _handState.HandScale;
      data.IsDataHighConfidence = isTracked && HandConfidence == TrackingConfidence.High;
      data.ShouldUseSystemGestureMaterial = IsSystemGestureInProgress;
    }

    return data;
  }

  OVRMesh.MeshType OVRMesh.IOVRMeshDataProvider.GetMeshType()
  {
    return HandType switch
    {
      HandTypes.None => OVRMesh.MeshType.None,
      HandTypes.HandLeft => OVRMesh.MeshType.HandLeft,
      HandTypes.HandRight => OVRMesh.MeshType.HandRight,
      _ => OVRMesh.MeshType.None,
    };
  }

  OVRMeshRenderer.MeshRendererData OVRMeshRenderer.IOVRMeshRendererDataProvider.GetMeshRendererData()
  {
    var data = new OVRMeshRenderer.MeshRendererData();

    data.IsDataValid = isDataValid;
    if (isDataValid)
    {
      data.IsDataHighConfidence = isTracked && HandConfidence == TrackingConfidence.High;
      data.ShouldUseSystemGestureMaterial = IsSystemGestureInProgress;
    }

    return data;
  }
}
