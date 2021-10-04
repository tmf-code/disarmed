using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class Skeleton : MonoBehaviour
{
  public interface ISkeletonDataProvider
  {
    SkeletonType GetSkeletonType();
    SkeletonPoseData GetSkeletonPoseData();
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

  public enum SkeletonType
  {
    None = OVRPlugin.SkeletonType.None,
    HandLeft = OVRPlugin.SkeletonType.HandLeft,
    HandRight = OVRPlugin.SkeletonType.HandRight,
  }

  public enum BoneId
  {
    Invalid = OVRPlugin.BoneId.Invalid,
    // hand bones
    Hand_Start = OVRPlugin.BoneId.Hand_Start,
    Hand_WristRoot = OVRPlugin.BoneId.Hand_WristRoot, // root frame of the hand, where the wrist is located
    Hand_ForearmStub = OVRPlugin.BoneId.Hand_ForearmStub, // frame for user's forearm
    Hand_Thumb0 = OVRPlugin.BoneId.Hand_Thumb0, // thumb trapezium bone
    Hand_Thumb1 = OVRPlugin.BoneId.Hand_Thumb1, // thumb metacarpal bone
    Hand_Thumb2 = OVRPlugin.BoneId.Hand_Thumb2, // thumb proximal phalange bone
    Hand_Thumb3 = OVRPlugin.BoneId.Hand_Thumb3, // thumb distal phalange bone
    Hand_Index1 = OVRPlugin.BoneId.Hand_Index1, // index proximal phalange bone
    Hand_Index2 = OVRPlugin.BoneId.Hand_Index2, // index intermediate phalange bone
    Hand_Index3 = OVRPlugin.BoneId.Hand_Index3, // index distal phalange bone
    Hand_Middle1 = OVRPlugin.BoneId.Hand_Middle1, // middle proximal phalange bone
    Hand_Middle2 = OVRPlugin.BoneId.Hand_Middle2, // middle intermediate phalange bone
    Hand_Middle3 = OVRPlugin.BoneId.Hand_Middle3, // middle distal phalange bone
    Hand_Ring1 = OVRPlugin.BoneId.Hand_Ring1, // ring proximal phalange bone
    Hand_Ring2 = OVRPlugin.BoneId.Hand_Ring2, // ring intermediate phalange bone
    Hand_Ring3 = OVRPlugin.BoneId.Hand_Ring3, // ring distal phalange bone
    Hand_Pinky0 = OVRPlugin.BoneId.Hand_Pinky0, // pinky metacarpal bone
    Hand_Pinky1 = OVRPlugin.BoneId.Hand_Pinky1, // pinky proximal phalange bone
    Hand_Pinky2 = OVRPlugin.BoneId.Hand_Pinky2, // pinky intermediate phalange bone
    Hand_Pinky3 = OVRPlugin.BoneId.Hand_Pinky3, // pinky distal phalange bone
    Hand_MaxSkinnable = OVRPlugin.BoneId.Hand_MaxSkinnable,
    // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
    // NOTE: Hand_ThumbTip == Hand_MaxSkinnable since the extended tips need to be contiguous
    Hand_ThumbTip = OVRPlugin.BoneId.Hand_ThumbTip, // tip of the thumb
    Hand_IndexTip = OVRPlugin.BoneId.Hand_IndexTip, // tip of the index finger
    Hand_MiddleTip = OVRPlugin.BoneId.Hand_MiddleTip, // tip of the middle finger
    Hand_RingTip = OVRPlugin.BoneId.Hand_RingTip, // tip of the ring finger
    Hand_PinkyTip = OVRPlugin.BoneId.Hand_PinkyTip, // tip of the pinky
    Hand_End = OVRPlugin.BoneId.Hand_End,
    // add new bones here

    Max = OVRPlugin.BoneId.Max
  }

  [SerializeField]
  protected SkeletonType _skeletonType = SkeletonType.None;
  [SerializeField]
  private ISkeletonDataProvider _dataProvider;

  [SerializeField]
  private bool _updateRootPose = false;

  public bool updateBones = false;

  [SerializeField]
  private bool _updateRootScale = false;
  [SerializeField]
  private bool _enablePhysicsCapsules = false;

  private GameObject _bonesGO;
  private GameObject _bindPosesGO;
  private GameObject _capsulesGO;

  protected List<Bone> _bones;
  private List<Bone> _bindPoses;
  private List<BoneCapsule> _capsules;

  protected OVRPlugin.Skeleton2 _skeleton = new OVRPlugin.Skeleton2();
  private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

  public bool IsInitialized { get; private set; }
  public bool IsDataValid { get; private set; }
  public bool IsDataHighConfidence { get; private set; }
  public IList<Bone> Bones { get; protected set; }
  public IList<Bone> BindPoses { get; private set; }
  public IList<BoneCapsule> Capsules { get; private set; }
  public SkeletonType GetSkeletonType()
  {
    return _skeletonType;
  }
  public int SkeletonChangedCount { get; private set; }

  private void Awake()
  {
    if (_dataProvider == null)
    {
      _dataProvider = GetComponent<ISkeletonDataProvider>();
    }

    _bones = new List<Bone>();
    Bones = _bones.AsReadOnly();

    _bindPoses = new List<Bone>();
    BindPoses = _bindPoses.AsReadOnly();

    _capsules = new List<BoneCapsule>();
    Capsules = _capsules.AsReadOnly();
  }

  private void Start()
  {
    if (ShouldInitialize())
    {
      Initialize();
    }
  }

  private bool ShouldInitialize()
  {
    if (IsInitialized)
    {
      return false;
    }

    if (_skeletonType == SkeletonType.None)
    {
      return false;
    }
    else if (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight)
    {
#if UNITY_EDITOR
      return OVRInput.IsControllerConnected(OVRInput.Controller.Hands);
#else
      return true;
#endif
    }
    else
    {
      return true;
    }
  }

  private void Initialize()
  {
    if (OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)_skeletonType, ref _skeleton))
    {
      InitializeBones();
      InitializeBindPose();
      InitializeCapsules();

      IsInitialized = true;
    }
  }

  protected virtual void InitializeBones()
  {
    bool flipX = (
        _skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight
    );

    if (!_bonesGO)
    {
      _bonesGO = new GameObject("Bones");
      _bonesGO.transform.SetParent(transform, false);
      _bonesGO.transform.localPosition = Vector3.zero;
      _bonesGO.transform.localRotation = Quaternion.identity;
    }

    if (_bones == null || _bones.Count != _skeleton.NumBones)
    {
      _bones = new List<Bone>(new Bone[_skeleton.NumBones]);
      Bones = _bones.AsReadOnly();
    }

    // pre-populate bones list before attempting to apply bone hierarchy
    for (int i = 0; i < _bones.Count; ++i)
    {
      Bone bone = _bones[i] ?? (_bones[i] = new Bone());
      bone.Id = (BoneId)_skeleton.Bones[i].Id;
      bone.ParentBoneIndex = _skeleton.Bones[i].ParentBoneIndex;

      Transform trans =
          bone.Transform != null
            ? bone.Transform
            : (
              bone.Transform =
                  new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id)).transform
          );
      trans.localPosition = flipX
          ? _skeleton.Bones[i].Pose.Position.FromFlippedXVector3f()
          : _skeleton.Bones[i].Pose.Position.FromFlippedZVector3f();
      trans.localRotation = flipX
          ? _skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf()
          : _skeleton.Bones[i].Pose.Orientation.FromFlippedZQuatf();
    }

    for (int i = 0; i < _bones.Count; ++i)
    {
      if ((BoneId)_bones[i].ParentBoneIndex == BoneId.Invalid)
      {
        _bones[i].Transform.SetParent(_bonesGO.transform, false);
      }
      else
      {
        _bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
      }
    }
  }

  private void InitializeBindPose()
  {
    if (!_bindPosesGO)
    {
      _bindPosesGO = new GameObject("BindPoses");
      _bindPosesGO.transform.SetParent(transform, false);
      _bindPosesGO.transform.localPosition = Vector3.zero;
      _bindPosesGO.transform.localRotation = Quaternion.identity;
    }

    if (_bindPoses == null || _bindPoses.Count != _bones.Count)
    {
      _bindPoses = new List<Bone>(new Bone[_bones.Count]);
      BindPoses = _bindPoses.AsReadOnly();
    }

    // pre-populate bones list before attempting to apply bone hierarchy
    for (int i = 0; i < _bindPoses.Count; ++i)
    {
      Bone bone = _bones[i];
      Bone bindPoseBone = _bindPoses[i] ?? (_bindPoses[i] = new Bone());
      bindPoseBone.Id = bone.Id;
      bindPoseBone.ParentBoneIndex = bone.ParentBoneIndex;

      Transform trans =
          bindPoseBone.Transform != null
            ? bindPoseBone.Transform
            : (bindPoseBone.Transform =
                new GameObject(
                    BoneLabelFromBoneId(_skeletonType, bindPoseBone.Id)
                ).transform
          );
      trans.localPosition = bone.Transform.localPosition;
      trans.localRotation = bone.Transform.localRotation;
    }

    for (int i = 0; i < _bindPoses.Count; ++i)
    {
      if ((BoneId)_bindPoses[i].ParentBoneIndex == BoneId.Invalid)
      {
        _bindPoses[i].Transform.SetParent(_bindPosesGO.transform, false);
      }
      else
      {
        _bindPoses[i].Transform.SetParent(
            _bindPoses[_bindPoses[i].ParentBoneIndex].Transform,
            false
        );
      }
    }
  }

  private void InitializeCapsules()
  {
    bool flipX = (
        _skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight
    );

    if (_enablePhysicsCapsules)
    {
      if (!_capsulesGO)
      {
        _capsulesGO = new GameObject("Capsules");
        _capsulesGO.transform.SetParent(transform, false);
        _capsulesGO.transform.localPosition = Vector3.zero;
        _capsulesGO.transform.localRotation = Quaternion.identity;
      }

      if (_capsules == null || _capsules.Count != _skeleton.NumBoneCapsules)
      {
        _capsules = new List<BoneCapsule>(new BoneCapsule[_skeleton.NumBoneCapsules]);
        Capsules = _capsules.AsReadOnly();
      }

      for (int i = 0; i < _capsules.Count; ++i)
      {
        Bone bone = _bones[_skeleton.BoneCapsules[i].BoneIndex];
        BoneCapsule capsule = _capsules[i] ?? (_capsules[i] = new BoneCapsule());
        capsule.BoneIndex = _skeleton.BoneCapsules[i].BoneIndex;

        if (capsule.CapsuleRigidbody == null)
        {
          capsule.CapsuleRigidbody = new GameObject(
              BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleRigidbody"
          ).AddComponent<Rigidbody>();
          capsule.CapsuleRigidbody.mass = 1.0f;
          capsule.CapsuleRigidbody.isKinematic = true;
          capsule.CapsuleRigidbody.useGravity = false;
          capsule.CapsuleRigidbody.collisionDetectionMode =
              CollisionDetectionMode.ContinuousSpeculative;
        }

        GameObject rbGO = capsule.CapsuleRigidbody.gameObject;
        rbGO.transform.SetParent(_capsulesGO.transform, false);
        rbGO.transform.SetPositionAndRotation(bone.Transform.position, bone.Transform.rotation);

        if (capsule.CapsuleCollider == null)
        {
          capsule.CapsuleCollider = new GameObject(
              BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleCollider"
          ).AddComponent<CapsuleCollider>();
          capsule.CapsuleCollider.isTrigger = false;
        }

        var start = flipX
            ? _skeleton.BoneCapsules[i].StartPoint.FromFlippedXVector3f()
            : _skeleton.BoneCapsules[i].StartPoint.FromFlippedZVector3f();
        var end = flipX
            ? _skeleton.BoneCapsules[i].EndPoint.FromFlippedXVector3f()
            : _skeleton.BoneCapsules[i].EndPoint.FromFlippedZVector3f();
        var delta = end - start;
        var magnitude = delta.magnitude;
        var rotation = Quaternion.FromToRotation(Vector3.right, delta);
        capsule.CapsuleCollider.radius = _skeleton.BoneCapsules[i].Radius;
        capsule.CapsuleCollider.height = magnitude + _skeleton.BoneCapsules[i].Radius * 2.0f;
        capsule.CapsuleCollider.direction = 0;
        capsule.CapsuleCollider.center = 0.5f * magnitude * Vector3.right;

        GameObject ccGO = capsule.CapsuleCollider.gameObject;
        ccGO.transform.SetParent(rbGO.transform, false);
        ccGO.transform.localPosition = start;
        ccGO.transform.localRotation = rotation;
      }
    }
  }

  private void Update()
  {
#if UNITY_EDITOR
    if (ShouldInitialize()) Initialize();
#endif

    if (!IsInitialized || _dataProvider == null)
    {
      IsDataValid = false;
      IsDataHighConfidence = false;
      return;
    }


    var data = _dataProvider.GetSkeletonPoseData();

    IsDataValid = data.IsDataValid;

    if (!data.IsDataValid) return;

    if (SkeletonChangedCount != data.SkeletonChangedCount)
    {
      SkeletonChangedCount = data.SkeletonChangedCount;
      IsInitialized = false;
      Initialize();
    }

    IsDataHighConfidence = data.IsDataHighConfidence;

    if (_updateRootPose)
    {
      transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
      transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
    }

    if (_updateRootScale)
    {
      transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
    }


    for (var i = 0; i < _bones.Count; ++i)
    {
      var bone = _bones[i];
      if (bone.Transform == null) continue;

      var isSkeletonTypeNone = _skeletonType == SkeletonType.None;
      if (isSkeletonTypeNone)
      {
        bone.localRotation = data.BoneRotations[i].FromFlippedZQuatf();
        continue;
      }

      var quaternion = data.BoneRotations[i].FromFlippedXQuatf();
      var isBoneWristRoot = bone.Id == BoneId.Hand_WristRoot;
      if (isBoneWristRoot)
      {
        quaternion *= wristFixupRotation;
      }
      bone.localRotation = quaternion;

      bone.Update(updateBones);
    }
  }

  private void FixedUpdate()
  {
    if (!IsInitialized || _dataProvider == null)
    {
      IsDataValid = false;
      IsDataHighConfidence = false;

      return;
    }

    Update();

    if (_enablePhysicsCapsules)
    {
      var data = _dataProvider.GetSkeletonPoseData();

      IsDataValid = data.IsDataValid;
      IsDataHighConfidence = data.IsDataHighConfidence;

      for (int i = 0; i < _capsules.Count; ++i)
      {
        BoneCapsule capsule = _capsules[i];
        var capsuleGO = capsule.CapsuleRigidbody.gameObject;

        if (data.IsDataValid && data.IsDataHighConfidence)
        {
          Transform bone = _bones[(int)capsule.BoneIndex].Transform;

          if (capsuleGO.activeSelf)
          {
            capsule.CapsuleRigidbody.MovePosition(bone.position);
            capsule.CapsuleRigidbody.MoveRotation(bone.rotation);
          }
          else
          {
            capsuleGO.SetActive(true);
            capsule.CapsuleRigidbody.position = bone.position;
            capsule.CapsuleRigidbody.rotation = bone.rotation;
          }
        }
        else
        {
          if (capsuleGO.activeSelf)
          {
            capsuleGO.SetActive(false);
          }
        }
      }
    }
  }

  public BoneId GetCurrentStartBoneId() => _skeletonType switch
  {
    SkeletonType.None => BoneId.Invalid,
    _ => BoneId.Hand_Start,
  };

  public BoneId GetCurrentEndBoneId() => _skeletonType switch
  {
    SkeletonType.None => BoneId.Invalid,
    _ => BoneId.Hand_End,
  };

  private BoneId GetCurrentMaxSkinnableBoneId() => _skeletonType switch
  {
    SkeletonType.None => BoneId.Invalid,
    _ => BoneId.Hand_MaxSkinnable,
  };

  public int GetCurrentNumBones() => _skeletonType switch
  {
    SkeletonType.None => 0,
    _ => GetCurrentEndBoneId() - GetCurrentStartBoneId(),
  };

  public int GetCurrentNumSkinnableBones() => _skeletonType switch
  {
    SkeletonType.None => 0,
    _ => GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId(),
  };

  // force aliased enum values to the more appropriate value
  public static string BoneLabelFromBoneId(SkeletonType skeletonType, BoneId boneId)
  {
    if (skeletonType == SkeletonType.None) return "Skeleton_Unknown";

    return boneId switch
    {
      BoneId.Hand_WristRoot => "Hand_WristRoot",
      BoneId.Hand_ForearmStub => "Hand_ForearmStub",
      BoneId.Hand_Thumb0 => "Hand_Thumb0",
      BoneId.Hand_Thumb1 => "Hand_Thumb1",
      BoneId.Hand_Thumb2 => "Hand_Thumb2",
      BoneId.Hand_Thumb3 => "Hand_Thumb3",
      BoneId.Hand_Index1 => "Hand_Index1",
      BoneId.Hand_Index2 => "Hand_Index2",
      BoneId.Hand_Index3 => "Hand_Index3",
      BoneId.Hand_Middle1 => "Hand_Middle1",
      BoneId.Hand_Middle2 => "Hand_Middle2",
      BoneId.Hand_Middle3 => "Hand_Middle3",
      BoneId.Hand_Ring1 => "Hand_Ring1",
      BoneId.Hand_Ring2 => "Hand_Ring2",
      BoneId.Hand_Ring3 => "Hand_Ring3",
      BoneId.Hand_Pinky0 => "Hand_Pinky0",
      BoneId.Hand_Pinky1 => "Hand_Pinky1",
      BoneId.Hand_Pinky2 => "Hand_Pinky2",
      BoneId.Hand_Pinky3 => "Hand_Pinky3",
      BoneId.Hand_ThumbTip => "Hand_ThumbTip",
      BoneId.Hand_IndexTip => "Hand_IndexTip",
      BoneId.Hand_MiddleTip => "Hand_MiddleTip",
      BoneId.Hand_RingTip => "Hand_RingTip",
      BoneId.Hand_PinkyTip => "Hand_PinkyTip",
      _ => "Hand_Unknown",
    };

  }
}

public class Bone
{
  public Skeleton.BoneId Id { get; set; }
  public short ParentBoneIndex { get; set; }
  public Transform Transform
  {
    get => transform; set
    {
      transform = value;
      Object.Destroy(copy);
      if (transform == null && transform.parent.transform == null) return;
      copy = Object.Instantiate(transform.gameObject, transform.parent.transform);
      copy.name = $"always_updates_{transform.name}";
      foreach (Transform child in copy.transform)
      {
        Object.DestroyImmediate(child.gameObject);
      }
    }
  }

  public Quaternion localRotation = Quaternion.identity;
  public GameObject copy;
  private Transform transform;

  public Bone() { }
  public Bone(Skeleton.BoneId id, short parentBoneIndex, Transform trans)
  {
    Id = id;
    ParentBoneIndex = parentBoneIndex;
    Transform = trans;
  }
  public void Update(bool updateMaster)
  {
    if (updateMaster)
    {
      Transform.localRotation = localRotation;
    }

    copy.transform.localPosition = transform.localPosition;
    copy.transform.localRotation = localRotation;
  }
}

public class BoneCapsule
{
  public short BoneIndex { get; set; }
  public Rigidbody CapsuleRigidbody { get; set; }
  public CapsuleCollider CapsuleCollider { get; set; }

  public BoneCapsule() { }

  public BoneCapsule(
      short boneIndex,
      Rigidbody capsuleRigidBody,
      CapsuleCollider capsuleCollider
  )
  {
    BoneIndex = boneIndex;
    CapsuleRigidbody = capsuleRigidBody;
    CapsuleCollider = capsuleCollider;
  }
}
