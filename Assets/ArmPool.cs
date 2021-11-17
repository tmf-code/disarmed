using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WorldArmBehaviour;

public class ArmPool : MonoBehaviour
{
  private readonly float stairHeight = 0.4F;
  private readonly float stairDepth = 0.4F;
  private readonly float startHeight = 0.4F;
  private readonly float startWidth = 1.7F;
  private readonly int maxStairCount = 6;
  private readonly float roomSize = 3.0F;


  private readonly float armSpacing = 0.7F;
  public WorldArmBehaviour leftArmPrefab;
  public WorldArmBehaviour rightArmPrefab;

  public Transform act4RainPoint;
  public Transform act4HandPoint1;
  public Transform act4HandPoint2;
  public Transform act4HandPoint3;
  public Transform act4HandPoint4;
  public Transform act4HandPoint5;
  public Transform act4HandPoint6;

  private List<List<Vector3>> positionsPerStair;
  private List<List<WorldArmBehaviour>> armsPerStair;

  public enum StairState
  {
    DuoArms,
    TwoCopy,
    All,
    VariableOffset,
    Flat,
    FlatRagdoll,
    RemoveStepOne,
    RemoveStepTwo,
    RemoveStepThree,
    TwoRecordedMovement,
    None,
    Act4,
    Act4ArmRain,
    Act4PlinthArmsFall,
  }

  private void SetStairCount(int nextStairCount, WorldArmBehaviours behaviour, PivotPoint.PivotPointType pivot)
  {
    for (int stairIndex = 0; stairIndex < maxStairCount; stairIndex++)
    {
      var spawnedObjects = armsPerStair[stairIndex];
      foreach (var item in spawnedObjects)
      {
        item.gameObject.SetActive(stairIndex < nextStairCount);
        item.behaviour = behaviour;
        item.GetComponent<PivotPoint>().pivotPointType = pivot;
      }
    }
  }

  public void SetStairState(StairState state)
  {
    switch (state)
    {
      case StairState.DuoArms:
        armsPerStair[1][9].gameObject.SetActive(true);
        armsPerStair[1][10].gameObject.SetActive(true);
        armsPerStair[1][9].behaviour = WorldArmBehaviours.CopyArmMovement;
        armsPerStair[1][10].behaviour = WorldArmBehaviours.CopyArmMovement;
        armsPerStair[1][9].GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Shoulder;
        armsPerStair[1][10].GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Shoulder;
        break;
      case StairState.TwoCopy:
        SetStairCount(2, WorldArmBehaviours.CopyArmMovement, PivotPoint.PivotPointType.Shoulder);
        break;
      case StairState.All:
        SetStairCount(6, WorldArmBehaviours.CopyArmMovement, PivotPoint.PivotPointType.Shoulder);
        break;
      case StairState.VariableOffset:
        SetStairCount(6, WorldArmBehaviours.CopyArmMovement, PivotPoint.PivotPointType.Shoulder);
        TimeOffsetPerStair();
        break;
      case StairState.Flat:
        FlattenOutOuterLevels();
        break;
      case StairState.FlatRagdoll:
        MakeOuterLevelsRagdoll();
        break;
      case StairState.RemoveStepOne:
        DisableStair(5);
        break;
      case StairState.RemoveStepTwo:
        DisableStair(4);
        break;
      case StairState.RemoveStepThree:
        DisableStair(3);
        break;
      case StairState.TwoRecordedMovement:
        SetStairCount(2, WorldArmBehaviours.MovementPlayback, PivotPoint.PivotPointType.ShoulderNoRotation);
        break;
      case StairState.Act4:
        SpawnInHand();
        break;
      case StairState.Act4ArmRain:

        SetStairCount(2, WorldArmBehaviours.MovementPlayback, PivotPoint.PivotPointType.ShoulderNoRotation);
        MoveOuterLevelsToRoomCenter();
        break;
      case StairState.Act4PlinthArmsFall:
        MakeOuterArmsFall();
        break;
      case StairState.None:
        SetStairCount(0, WorldArmBehaviours.MovementPlayback, PivotPoint.PivotPointType.Wrist);
        break;
    }
  }

  private void MakeOuterArmsFall()
  {
    for (int stairIndex = 0; stairIndex < maxStairCount; stairIndex++)
    {
      if (stairIndex > 2) continue;
      var spawnedObjects = armsPerStair[stairIndex];
      foreach (var item in spawnedObjects)
      {
        item.gameObject.SetActive(true);
        item.behaviour = WorldArmBehaviours.Ragdoll;
        item.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
      }
    }
  }

  public void MoveOuterLevelsToRoomCenter()
  {
    var startLevel = 2;

    var count = 0;
    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      for (var armIndex = 0; armIndex < armsPerStair[stairLevel].Count; armIndex++)
      {
        count += 1;
        var arm = armsPerStair[stairLevel][armIndex];
        if (count <= 6) continue;
        var delay = 0.4F * (stairLevel + 1F) * (armIndex + 1F);
        if (delay > 25F) continue;
        StartCoroutine(SpawnAtRainPoint(arm, delay));
      }
    }
  }

  public void SpawnInHand()
  {
    var startLevel = 2;

    var count = 0;
    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      for (var armIndex = 0; armIndex < armsPerStair[stairLevel].Count; armIndex++)
      {
        count += 1;
        var arm = armsPerStair[stairLevel][armIndex];
        if (count > 6) continue;

        var position = (count switch
        {
          1 => act4HandPoint1,
          2 => act4HandPoint2,
          3 => act4HandPoint3,
          4 => act4HandPoint4,
          5 => act4HandPoint5,
          6 => act4HandPoint6,
          _ => throw new System.NotImplementedException(),
        }).transform.position;

        arm.transform.position = position;
        arm.gameObject.SetActive(true);
        arm.behaviour = WorldArmBehaviours.MovementPlaybackRagdoll;
        arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Wrist;
      }
    }
  }

  public IEnumerator SpawnAtRainPoint(WorldArmBehaviour arm, float delay)
  {
    yield return new WaitForSeconds(delay);
    var position = act4RainPoint.transform.position;
    arm.transform.localPosition = position;
    arm.gameObject.SetActive(true);
    arm.behaviour = WorldArmBehaviours.MovementPlaybackRagdoll;
    arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
  }

  public void TimeOffsetPerStair()
  {
    for (int stairLevel = 0; stairLevel < maxStairCount; stairLevel++)
    {
      foreach (var stair in armsPerStair[stairLevel])
      {
        if (stair.TryGetComponent<CopyArmMovement>(out var copyArmMovement))
        {
          copyArmMovement.frameDelay = 10 * stairLevel;
        }
      }
    }
  }


  public void LongTimeOffset()
  {
    for (int stairLevel = 0; stairLevel < maxStairCount; stairLevel++)
    {
      foreach (var stair in armsPerStair[stairLevel])
      {
        if (stair.TryGetComponent<CopyArmMovement>(out var copyArmMovement))
        {
          copyArmMovement.frameDelay = 120;
        }
      }
    }
  }

  public void DisableStair(int stairLevel)
  {
    var stair = armsPerStair[stairLevel];

    foreach (var arm in stair)
    {
      arm.gameObject.SetActive(false);
    }
  }

  public void MakeOuterLevelsRagdoll()
  {
    var startLevel = 2;

    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      if (stairLevel >= 5) continue;

      foreach (var arm in armsPerStair[stairLevel])
      {
        arm.gameObject.SetActive(true);
        arm.behaviour = WorldArmBehaviours.Ragdoll;
        arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
        arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
      }
    }
  }

  public void FlattenOutOuterLevels()
  {
    var startLevel = 2;

    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      if (stairLevel >= 5) continue;
      foreach (var arm in armsPerStair[stairLevel])
      {
        var v = new Vector2(arm.transform.position.x, arm.transform.position.z);
        var rad = v.magnitude;
        var angle = Mathf.Atan2(v.y, v.x);
        var boost = 0.4F * (stairLevel - 1);
        var jitterX = Random.Range(0.9F, 1.1F);
        var jitterY = Random.Range(0.9F, 1.1F);
        var newRad = rad + boost;
        var newZ = newRad * Mathf.Sin(angle) * jitterX;
        var newX = newRad * Mathf.Cos(angle) * jitterY;
        var y = 1.4F;
        arm.transform.position = new Vector3(newX, y, newZ);
        arm.gameObject.SetActive(true);
        arm.behaviour = WorldArmBehaviours.MovementPlayback;
        arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.Shoulder;
      }
    }
  }

  void Awake()
  {
    positionsPerStair = GetSpawnPositions();
    armsPerStair = positionsPerStair.Select(value => new List<WorldArmBehaviour>()).ToList();

    for (var stairIndex = 0; stairIndex < positionsPerStair.Count; stairIndex++)
    {
      SpawnStairHands(stairIndex);
    }
  }

  private void SpawnStairHands(int stairIndex)
  {
    var stairPositions = positionsPerStair[stairIndex];
    var stairObjects = armsPerStair[stairIndex];

    for (var stairPositionIndex = 0; stairPositionIndex < stairPositions.Count; stairPositionIndex++)
    {
      var position = stairPositions[stairPositionIndex];
      var arm = position.x < 0.0 ? Instantiate(rightArmPrefab) : Instantiate(leftArmPrefab);
      arm.transform.parent = transform;
      arm.transform.position = position;
      arm.transform.localRotation = Quaternion.Euler(0, 180F, 0);
      arm.behaviour = WorldArmBehaviours.CopyArmMovement;
      arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.ShoulderNoRotation;
      stairObjects.Add(arm);
      arm.gameObject.SetActive(false);
    }
  }

  public Bounds GetBounds(int stair)
  {
    var bounds = new Bounds();
    bounds.center = new Vector3(0, startHeight + stair * stairHeight + 0.5F, 0) + transform.position;
    bounds.extents = new Vector3(startWidth + stair * stairDepth * 2F + stairDepth, 0, startWidth + stair * stairDepth * 2F + stairDepth);

    return bounds;
  }

  public List<List<Vector3>> GetSpawnPositions()
  {
    var result = new List<List<Vector3>>();
    for (int i = 0; i < maxStairCount; i++)
    {
      var bounds = GetBounds(i);
      var armsPerStair = (int)Mathf.Floor(bounds.extents.x / armSpacing);

      var stair = new List<Vector3>();

      for (int j = 0; j < armsPerStair; j++)
      {
        var z = -bounds.extents.x / 2F + armSpacing * j + armSpacing / 2F;
        var y = 0;
        var x = -bounds.extents.z / 2.0F;


        var position = new Vector3(x, y, z) + bounds.center;
        if (position.z < -roomSize / 2 + armSpacing) continue;
        stair.Add(position);
      }

      for (int j = 0; j < armsPerStair; j++)
      {
        var z = -bounds.extents.x / 2F + armSpacing * j + armSpacing / 2F;
        var y = 0;
        var x = bounds.extents.z / 2.0F;


        var position = new Vector3(x, y, z) + bounds.center;
        if (position.z < -roomSize / 2 + armSpacing) continue;
        stair.Add(position);
      }

      for (int j = 0; j < armsPerStair; j++)
      {
        var x = -bounds.extents.x / 2F + armSpacing * j + armSpacing / 2F;
        var y = 0;
        var z = bounds.extents.z / 2.0F;
        var position = new Vector3(x, y, z) + bounds.center;
        stair.Add(position);
      }
      result.Add(stair);
    }
    return result;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.green;

    for (int i = 0; i < maxStairCount; i++)
    {
      var bounds = GetBounds(i);
      Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }

    var spawnPositionsPerStair = GetSpawnPositions();

    for (var stairLevel = 0; stairLevel < spawnPositionsPerStair.Count; stairLevel++)
    {
      var stair = spawnPositionsPerStair[stairLevel];
      for (var positionIndex = 0; positionIndex < stair.Count; positionIndex++)
      {
        if (positionIndex == 0)
        {
          Gizmos.color = Color.red;
        }
        else if (positionIndex % 2 == 0)
        {
          Gizmos.color = Color.black;
        }
        else
        {
          Gizmos.color = Color.green;
        }
        var position = stair[positionIndex];
        Gizmos.DrawSphere(position, 0.1F);
      }
    }
  }
}