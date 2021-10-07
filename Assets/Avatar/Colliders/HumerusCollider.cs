public class HumerusCollider : BoneCollider
{
  new void Start()
  {
    tag = "Humerus";
    var handType = gameObject.GetComponent<Handedness>().handType;
    boneStart = handType == HandTypes.HandLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    boneEnd = handType == HandTypes.HandLeft ? BoneName.b_l_shoulder : BoneName.b_r_shoulder;
    base.Start();
  }
}
