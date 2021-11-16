using UnityEngine;

public class ApplyBigArmMovement : UpperHandMovement
{
  public override ObjectToFramesDictionary GetRecording()
  {
    return GameObject.Find("Recordings").GetComponentOrThrow<RecordingsStore>().upperHandRight.UnwrapOrLoad();
  }
}
