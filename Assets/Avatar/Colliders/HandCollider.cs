public class HandCollider : BoneCollider
{
  new void Start()
  {
    tag = "Hand";
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == CustomHand.HandTypes.HandLeft ? BoneName.b_l_wrist : BoneName.b_r_wrist;
    boneEnd = handType == CustomHand.HandTypes.HandLeft ? BoneName.b_l_middle_null : BoneName.b_r_middle_null;
    base.Start();
  }
}
