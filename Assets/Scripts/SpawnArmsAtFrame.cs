using UnityEngine;

public class SpawnArmsAtFrame : MonoBehaviour
{
  public int frame = 0;
  private ArmPlayback armsPlayback;
  public GameObject arms;
  public bool spawned = false;
  // Start is called before the first frame update
  void Start()
  {
    armsPlayback = gameObject.GetComponentOrThrow<ArmPlayback>();
  }

  // Update is called once per frame
  void Update()
  {
    var shouldSpawn = armsPlayback.framesPlayed >= frame;
    if (!shouldSpawn) return;
    if (!spawned)
    {
      Instantiate(arms);
      arms.transform.position = transform.FindRecursiveOrThrow("l_palm_center_marker").transform.position;
      arms.transform.Translate(Vector3.up * 0.4F);
      spawned = true;
    }
    else if (armsPlayback.framesPlayed < frame + 20)
    {
      arms.transform.position = transform.FindRecursiveOrThrow("l_palm_center_marker").transform.position;
      arms.transform.Translate(Vector3.up * 0.4F);
    }
  }
}
