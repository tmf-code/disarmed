public class HumerusCollider : RemoteBoneCollider
{
  new void Start()
  {
    tag = "Humerus";
    var isLeft = gameObject.GetComponent<Handedness>().IsLeft();
    boneStart = isLeft ? BoneName.b_l_humerus : BoneName.b_r_humerus;
    boneEnd = isLeft ? BoneName.b_l_shoulder : BoneName.b_r_shoulder;
    base.Start();
  }
}
