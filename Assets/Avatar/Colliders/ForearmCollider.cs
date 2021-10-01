public class ForearmCollider : BoneCollider
{
  new void Start()
  {
    tag = "Forearm";
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_forearm_stub : BoneName.b_r_forearm_stub;
    boneEnd = handType == OVRHand.Hand.HandLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    base.Start();
  }
}
