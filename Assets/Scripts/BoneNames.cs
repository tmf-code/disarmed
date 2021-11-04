using System;

public enum BoneName
{
  None,
  b_l_wrist,
  b_l_forearm_stub,
  b_l_humerus,
  b_l_shoulder,
  b_l_shoulder_end,
  b_l_index1,
  b_l_index2,
  b_l_index3,
  b_l_index_null,
  b_l_index_null_end,
  l_index_finger_pad_marker,
  l_index_finger_tip_marker,
  l_index_fingernail_marker,
  l_index_dip_fe_axis_marker,
  l_index_pip_fe_axis_marker,
  b_l_middle1,
  b_l_middle2,
  b_l_middle3,
  b_l_middle_null,
  b_l_middle_null_end,
  l_middle_finger_pad_marker,
  l_middle_finger_tip_marker,
  l_middle_fingernail_marker,
  l_middle_dip_fe_axis_marker,
  l_middle_knuckle_marker,
  l_middle_pip_fe_axis_marker,
  b_l_pinky0,
  b_l_pinky1,
  b_l_pinky2,
  b_l_pinky3,
  b_l_pinky_null,
  b_l_pinky_null_end,
  l_pinky_finger_pad_marker,
  l_pinky_finger_tip_marker,
  l_pinky_fingernail_marker,
  l_pinky_dip_fe_axis_marker,
  l_pinky_pip_fe_axis_marker,
  b_l_ring1,
  b_l_ring2,
  b_l_ring3,
  b_l_ring_null,
  b_l_ring_null_end,
  l_ring_finger_pad_marker,
  l_ring_finger_tip_marker,
  l_ring_fingernail_marker,
  l_ring_dip_fe_axis_marker,
  l_ring_pip_fe_axis_marker,
  b_l_thumb0,
  b_l_thumb1,
  b_l_thumb2,
  b_l_thumb3,
  b_l_thumb_null,
  b_l_thumb_null_end,
  l_thumb_finger_pad_marker,
  l_thumb_finger_tip_marker,
  l_thumb_fingernail_marker,
  l_thumb_ip_fe_axis_marker,
  l_thumb_knuckle_marker,
  l_thumb_mcp_fe_axis_marker,
  l_thumb_palm_knuckle_marker,
  l_index_knuckle_marker,
  l_index_mcp_aa_axis_marker,
  l_index_mcp_fe_axis_marker,
  l_index_palm_collider_base0_marker,
  l_index_palm_collider_base1_marker,
  l_index_palm_knuckle_marker,
  l_middle_mcp_aa_axis_marker,
  l_middle_mcp_fe_axis_marker,
  l_middle_palm_collider_base0_marker,
  l_middle_palm_collider_base1_marker,
  l_middle_palm_knuckle_marker,
  l_palm_center_marker,
  l_pinky_knuckle_marker,
  l_pinky_mcp_aa_axis_marker,
  l_pinky_mcp_fe_axis_marker,
  l_pinky_palm_collider_base0_marker,
  l_pinky_palm_collider_base1_marker,
  l_pinky_palm_knuckle_marker,
  l_ring_knuckle_marker,
  l_ring_mcp_aa_axis_marker,
  l_ring_mcp_fe_axis_marker,
  l_ring_palm_collider_base0_marker,
  l_ring_palm_collider_base1_marker,
  l_ring_palm_knuckle_marker,
  l_thumb_cmc_aa_axis_marker,
  l_thumb_cmc_fe_axis_marker,
  b_r_wrist,
  b_r_forearm_stub,
  b_r_humerus,
  b_r_shoulder,
  b_r_shoulder_end,
  b_r_index1,
  b_r_index2,
  b_r_index3,
  b_r_index_null,
  b_r_index_null_end,
  r_index_finger_pad_marker,
  r_index_finger_tip_marker,
  r_index_fingernail_marker,
  r_index_dip_fe_axis_marker,
  r_index_pip_fe_axis_marker,
  b_r_middle1,
  b_r_middle2,
  b_r_middle3,
  b_r_middle_null,
  b_r_middle_null_end,
  r_middle_finger_pad_marker,
  r_middle_finger_tip_marker,
  r_middle_fingernail_marker,
  r_middle_dip_fe_axis_marker,
  r_middle_knuckle_marker,
  r_middle_pip_fe_axis_marker,
  b_r_pinky0,
  b_r_pinky1,
  b_r_pinky2,
  b_r_pinky3,
  b_r_pinky_null,
  b_r_pinky_null_end,
  r_pinky_finger_pad_marker,
  r_pinky_finger_tip_marker,
  r_pinky_fingernail_marker,
  r_pinky_dip_fe_axis_marker,
  r_pinky_pip_fe_axis_marker,
  b_r_ring1,
  b_r_ring2,
  b_r_ring3,
  b_r_ring_null,
  b_r_ring_null_end,
  r_ring_finger_pad_marker,
  r_ring_finger_tip_marker,
  r_ring_fingernail_marker,
  r_ring_dip_fe_axis_marker,
  r_ring_pip_fe_axis_marker,
  b_r_thumb0,
  b_r_thumb1,
  b_r_thumb2,
  b_r_thumb3,
  b_r_thumb_null,
  b_r_thumb_null_end,
  r_thumb_finger_pad_marker,
  r_thumb_finger_tip_marker,
  r_thumb_fingernail_marker,
  r_thumb_ip_fe_axis_marker,
  r_thumb_knuckle_marker,
  r_thumb_mcp_fe_axis_marker,
  r_thumb_palm_knuckle_marker,
  r_index_knuckle_marker,
  r_index_mcp_aa_axis_marker,
  r_index_mcp_fe_axis_marker,
  r_index_palm_collider_base0_marker,
  r_index_palm_collider_base1_marker,
  r_index_palm_knuckle_marker,
  r_middle_mcp_aa_axis_marker,
  r_middle_mcp_fe_axis_marker,
  r_middle_palm_collider_base0_marker,
  r_middle_palm_collider_base1_marker,
  r_middle_palm_knuckle_marker,
  r_palm_center_marker,
  r_pinky_knuckle_marker,
  r_pinky_mcp_aa_axis_marker,
  r_pinky_mcp_fe_axis_marker,
  r_pinky_palm_collider_base0_marker,
  r_pinky_palm_collider_base1_marker,
  r_pinky_palm_knuckle_marker,
  r_ring_knuckle_marker,
  r_ring_mcp_aa_axis_marker,
  r_ring_mcp_fe_axis_marker,
  r_ring_palm_collider_base0_marker,
  r_ring_palm_collider_base1_marker,
  r_ring_palm_knuckle_marker,
  r_thumb_cmc_aa_axis_marker,
  r_thumb_cmc_fe_axis_marker,
}

public enum TrackedBones
{
  // hand bones
  Hand_WristRoot, // root frame of the hand, where the wrist is located
  Hand_ForearmStub, // frame for user's forearm
  Hand_Thumb0, // thumb trapezium bone
  Hand_Thumb1, // thumb metacarpal bone
  Hand_Thumb2, // thumb proximal phalange bone
  Hand_Thumb3, // thumb distal phalange bone
  Hand_Index1, // index proximal phalange bone
  Hand_Index2, // index intermediate phalange bone
  Hand_Index3, // index distal phalange bone
  Hand_Middle1, // middle proximal phalange bone
  Hand_Middle2, // middle intermediate phalange bone
  Hand_Middle3, // middle distal phalange bone
  Hand_Ring1, // ring proximal phalange bone
  Hand_Ring2, // ring intermediate phalange bone
  Hand_Ring3, // ring distal phalange bone
  Hand_Pinky0, // pinky metacarpal bone
  Hand_Pinky1, // pinky proximal phalange bone
  Hand_Pinky2, // pinky intermediate phalange bone
  Hand_Pinky3, // pinky distal phalange bone

  // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
  Hand_ThumbTip, // tip of the thumb
  Hand_IndexTip, // tip of the index finger
  Hand_MiddleTip, // tip of the middle finger
  Hand_RingTip, // tip of the ring finger
  Hand_PinkyTip, // tip of the pinky
  Hand_End,
}


public class BoneNameToBoneId
{
  public static TrackedBones? GetTrackedBone(BoneName boneName)
  {
    TrackedBones? result = boneName switch
    {
      BoneName.b_l_wrist => TrackedBones.Hand_WristRoot,
      BoneName.b_l_forearm_stub => TrackedBones.Hand_ForearmStub,
      BoneName.b_l_thumb0 => TrackedBones.Hand_Thumb0,
      BoneName.b_l_thumb1 => TrackedBones.Hand_Thumb1,
      BoneName.b_l_thumb2 => TrackedBones.Hand_Thumb2,
      BoneName.b_l_thumb3 => TrackedBones.Hand_Thumb3,
      BoneName.b_l_index1 => TrackedBones.Hand_Index1,
      BoneName.b_l_index2 => TrackedBones.Hand_Index2,
      BoneName.b_l_index3 => TrackedBones.Hand_Index3,
      BoneName.b_l_middle1 => TrackedBones.Hand_Middle1,
      BoneName.b_l_middle2 => TrackedBones.Hand_Middle2,
      BoneName.b_l_middle3 => TrackedBones.Hand_Middle3,
      BoneName.b_l_ring1 => TrackedBones.Hand_Ring1,
      BoneName.b_l_ring2 => TrackedBones.Hand_Ring2,
      BoneName.b_l_ring3 => TrackedBones.Hand_Ring3,
      BoneName.b_l_pinky0 => TrackedBones.Hand_Pinky0,
      BoneName.b_l_pinky1 => TrackedBones.Hand_Pinky1,
      BoneName.b_l_pinky2 => TrackedBones.Hand_Pinky2,
      BoneName.b_l_pinky3 => TrackedBones.Hand_Pinky3,
      BoneName.l_thumb_finger_tip_marker => TrackedBones.Hand_ThumbTip,
      BoneName.l_index_finger_tip_marker => TrackedBones.Hand_IndexTip,
      BoneName.l_middle_finger_tip_marker => TrackedBones.Hand_MiddleTip,
      BoneName.l_ring_finger_tip_marker => TrackedBones.Hand_RingTip,
      BoneName.l_pinky_finger_tip_marker => TrackedBones.Hand_PinkyTip,
      BoneName.b_r_wrist => TrackedBones.Hand_WristRoot,
      BoneName.b_r_forearm_stub => TrackedBones.Hand_ForearmStub,
      BoneName.b_r_thumb0 => TrackedBones.Hand_Thumb0,
      BoneName.b_r_thumb1 => TrackedBones.Hand_Thumb1,
      BoneName.b_r_thumb2 => TrackedBones.Hand_Thumb2,
      BoneName.b_r_thumb3 => TrackedBones.Hand_Thumb3,
      BoneName.b_r_index1 => TrackedBones.Hand_Index1,
      BoneName.b_r_index2 => TrackedBones.Hand_Index2,
      BoneName.b_r_index3 => TrackedBones.Hand_Index3,
      BoneName.b_r_middle1 => TrackedBones.Hand_Middle1,
      BoneName.b_r_middle2 => TrackedBones.Hand_Middle2,
      BoneName.b_r_middle3 => TrackedBones.Hand_Middle3,
      BoneName.b_r_ring1 => TrackedBones.Hand_Ring1,
      BoneName.b_r_ring2 => TrackedBones.Hand_Ring2,
      BoneName.b_r_ring3 => TrackedBones.Hand_Ring3,
      BoneName.b_r_pinky0 => TrackedBones.Hand_Pinky0,
      BoneName.b_r_pinky1 => TrackedBones.Hand_Pinky1,
      BoneName.b_r_pinky2 => TrackedBones.Hand_Pinky2,
      BoneName.b_r_pinky3 => TrackedBones.Hand_Pinky3,
      BoneName.r_thumb_finger_tip_marker => TrackedBones.Hand_ThumbTip,
      BoneName.r_index_finger_tip_marker => TrackedBones.Hand_IndexTip,
      BoneName.r_middle_finger_tip_marker => TrackedBones.Hand_MiddleTip,
      BoneName.r_ring_finger_tip_marker => TrackedBones.Hand_RingTip,
      BoneName.r_pinky_finger_tip_marker => TrackedBones.Hand_PinkyTip,
      _ => null,
    };

    return result;
  }
  public static bool IsTrackedBone(BoneName boneName) => GetTrackedBone(boneName).HasValue;

  public static bool IsTrackedBone(string name)
  {
    if (!Enum.TryParse<BoneName>(name, out var boneName)) return false;
    if (!IsTrackedBone(boneName)) return false;

    return true;
  }
}