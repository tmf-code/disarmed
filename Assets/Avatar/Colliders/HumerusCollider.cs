public class HumerusCollider : BoneCollider
{
  new void Start()
  {
    tag = "Humerus";
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == CustomHand.HandTypes.HandLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    boneEnd = handType == CustomHand.HandTypes.HandLeft ? BoneName.b_l_shoulder : BoneName.b_r_shoulder;
    base.Start();
  }
}
