public class HandCollider : BoneCollider
{
  new void Start()
  {
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_wrist : BoneName.b_r_wrist;
    boneEnd = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_middle_null : BoneName.b_r_middle_null;
    base.Start();
  }
}
