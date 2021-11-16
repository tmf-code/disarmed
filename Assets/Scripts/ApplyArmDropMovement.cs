using UnityEngine;

public class ApplyArmDropMovement : UpperHandMovement
{
  public override ObjectToFramesDictionary GetRecording()
  {
    return GameObject.Find("Recordings")
      .GetComponentOrThrow<RecordingsStore>().act4ArmDropRight
      .UnwrapOrLoad()
      .GetValue(Act4ArmDropRight.armDrop2Right)
      .Unwrap();
  }
}
