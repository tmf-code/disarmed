public class ForearmCollider : BoneCollider
{
  new void Start()
  {
    tag = "Forearm";
    var isLeft = gameObject.GetComponent<Handedness>().IsLeft();
    boneStart = isLeft ? BoneName.b_l_forearm_stub : BoneName.b_r_forearm_stub;
    boneEnd = isLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    base.Start();
  }
}