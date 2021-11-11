using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtraStairsSpawner : MonoBehaviour
{
  [ShowOnly] private readonly float stairHeight = 0.4F;
  [ShowOnly] private readonly float stairDepth = 0.4F;
  [ShowOnly] private readonly float startHeight = 0.4F;
  [ShowOnly] private readonly float startWidth = 1.4F;
  [ShowOnly] private readonly int maxStairCount = 8;
  [ShowOnly] private readonly float roomSize = 3.0F;


  [ShowOnly] public float armSpacing = 0.8F;
  public ArmBehaviour leftArmPrefab;
  public ArmBehaviour rightArmPrefab;
  private List<List<Vector3>> positionsPerStair;
  private List<List<ArmBehaviour>> spawnedObjectsPerStair;

  public void SetStairCount(int nextStairCount, ArmBehaviour.ArmBehaviorType behaviour, PivotPoint.PivotPointType pivot)
  {
    for (int stairIndex = 0; stairIndex < maxStairCount; stairIndex++)
    {
      var spawnedObjects = spawnedObjectsPerStair[stairIndex];
      foreach (var item in spawnedObjects)
      {
        item.gameObject.SetActive(stairIndex < nextStairCount);
        item.behavior = behaviour;
        item.GetComponent<PivotPoint>().pivotPointType = pivot;
      }
    }
  }

  void Start()
  {
    positionsPerStair = GetSpawnPositions();
    spawnedObjectsPerStair = positionsPerStair.Select(value => new List<ArmBehaviour>()).ToList();

    for (var stairIndex = 0; stairIndex < positionsPerStair.Count; stairIndex++)
    {
      SpawnStairHands(stairIndex);
    }
  }

  private void SpawnStairHands(int stairIndex)
  {
    var stairPositions = positionsPerStair[stairIndex];
    var stairObjects = spawnedObjectsPerStair[stairIndex];

    for (var stairPositionIndex = 0; stairPositionIndex < stairPositions.Count; stairPositionIndex++)
    {
      var position = stairPositions[stairPositionIndex];
      var arm = Random.value > 0.5 ? Instantiate(leftArmPrefab) : Instantiate(rightArmPrefab);
      arm.transform.parent = transform;
      arm.transform.position = position;
      arm.behavior = ArmBehaviour.ArmBehaviorType.CopyArmMovement;
      arm.owner = ArmBehaviour.ArmOwnerType.World;
      arm.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.ShoulderNoRotation;
      stairObjects.Add(arm);
      arm.gameObject.SetActive(false);
    }
  }

  public Bounds GetBounds(int stair)
  {
    var bounds = new Bounds();
    bounds.center = new Vector3(0, startHeight + stair * stairHeight, 0) + transform.position;
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

    foreach (var stair in spawnPositionsPerStair)
    {
      foreach (var position in stair)
      {
        Gizmos.DrawSphere(position, 0.1F);
      }
    }
  }
}