public class HandCollider : RemoteBoneCollider
{
  new void Start()
  {
    tag = "Hand";
    var isLeft = gameObject.GetComponent<Handedness>().IsLeft();
    boneStart = isLeft ? BoneName.b_l_wrist : BoneName.b_r_wrist;
    boneEnd = isLeft ? BoneName.b_l_middle_null : BoneName.b_r_middle_null;
    base.Start();
  }
}
