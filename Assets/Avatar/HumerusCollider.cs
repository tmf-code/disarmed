public class HumerusCollider : BoneCollider
{
  new void Start()
  {
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    boneEnd = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_shoulder : BoneName.b_r_shoulder;
    base.Start();
  }
}
