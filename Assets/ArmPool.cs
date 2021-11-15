using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmPool : MonoBehaviour
{
  private readonly float stairHeight = 0.4F;
  private readonly float stairDepth = 0.4F;
  private readonly float startHeight = 0.4F;
  private readonly float startWidth = 2F;
  private readonly int maxStairCount = 8;
  private readonly float roomSize = 3.0F;


  private readonly float armSpacing = 0.8F;
  public ArmBehaviour leftArmPrefab;
  public ArmBehaviour rightArmPrefab;
  private List<List<Vector3>> positionsPerStair;
  private List<List<ArmBehaviour>> spawnedObjectsPerStair;

  public enum StairState
  {
    TwoCopy,
    All,
    VariableOffset,
    LongTimeOffset,
    Flat,
    FlatRagdoll,
    RemoveStepOne,
    RemoveStepTwo,
    RemoveStepThree,
    TwoRecordedMovement,
    None,
    Act4,
  }

  private void SetStairCount(int nextStairCount, ArmBehaviour.ArmBehaviorType behaviour, PivotPoint.PivotPointType pivot)
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

  public void SetStairState(StairState state)
  {
    switch (state)
    {
      case StairState.TwoCopy:
        SetStairCount(2, ArmBehaviour.ArmBehaviorType.CopyArmMovement, PivotPoint.PivotPointType.ShoulderNoRotation);
        break;
      case StairState.All:
        SetStairCount(8, ArmBehaviour.ArmBehaviorType.CopyArmMovement, PivotPoint.PivotPointType.ShoulderNoRotation);
        break;
      case StairState.VariableOffset:
        SetStairCount(8, ArmBehaviour.ArmBehaviorType.CopyArmMovement, PivotPoint.PivotPointType.ShoulderNoRotation);
        TimeOffsetPerStair();
        break;
      case StairState.LongTimeOffset:
        SetStairCount(8, ArmBehaviour.ArmBehaviorType.CopyArmMovement, PivotPoint.PivotPointType.ShoulderNoRotation);
        LongTimeOffset();
        break;
      case StairState.Flat:
        FlattenOutOuterLevels();
        break;
      case StairState.FlatRagdoll:
        MakeOuterLevelsRagdoll();
        break;
      case StairState.RemoveStepOne:
        DisableStair(7);
        break;
      case StairState.RemoveStepTwo:
        DisableStair(6);
        DisableStair(5);
        break;
      case StairState.RemoveStepThree:
        DisableStair(4);
        DisableStair(3);
        break;
      case StairState.TwoRecordedMovement:
        SetStairCount(2, ArmBehaviour.ArmBehaviorType.MovementPlayback, PivotPoint.PivotPointType.ShoulderNoRotation);
        break;
      case StairState.Act4:
        SetStairCount(2, ArmBehaviour.ArmBehaviorType.MovementPlayback, PivotPoint.PivotPointType.ShoulderNoRotation);
        MoveOuterLevelsToRoomCenter();

        break;
      case StairState.None:
        SetStairCount(0, ArmBehaviour.ArmBehaviorType.MovementPlayback, PivotPoint.PivotPointType.Wrist);
        break;
    }
  }

  public void MoveOuterLevelsToRoomCenter()
  {
    var startLevel = 2;

    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      foreach (var stair in spawnedObjectsPerStair[stairLevel])
      {
        var position = new Vector3(Random.Range(-0.5F, 0.5F) * roomSize, startHeight, Random.Range(-0.5F, 0.5F) * roomSize);
        stair.transform.localPosition = position;
        stair.gameObject.SetActive(true);
        stair.behavior = ArmBehaviour.ArmBehaviorType.MovementPlaybackRagdoll;
        stair.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
      }
    }
  }

  public void TimeOffsetPerStair()
  {
    for (int stairLevel = 0; stairLevel < maxStairCount; stairLevel++)
    {
      foreach (var stair in spawnedObjectsPerStair[stairLevel])
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
      foreach (var stair in spawnedObjectsPerStair[stairLevel])
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
    var stair = spawnedObjectsPerStair[stairLevel];

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
      foreach (var stair in spawnedObjectsPerStair[stairLevel])
      {
        stair.gameObject.SetActive(true);
        stair.behavior = ArmBehaviour.ArmBehaviorType.Ragdoll;
        stair.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.None;
      }
    }
  }

  public void FlattenOutOuterLevels()
  {
    var startLevel = 2;

    for (int stairLevel = startLevel; stairLevel < maxStairCount; stairLevel++)
    {
      if (stairLevel >= 5) continue;
      foreach (var stair in spawnedObjectsPerStair[stairLevel])
      {
        var v = new Vector2(stair.transform.position.x, stair.transform.position.z);
        var rad = v.magnitude;
        var angle = Mathf.Atan2(v.y, v.x);
        var boost = 0.4F * (stairLevel - 1);
        var jitterX = Random.Range(0.9F, 1.1F);
        var jitterY = Random.Range(0.9F, 1.1F);
        var newRad = rad + boost;
        var newZ = newRad * Mathf.Sin(angle) * jitterX;
        var newX = newRad * Mathf.Cos(angle) * jitterY;
        var y = 0.8F;
        stair.transform.position = new Vector3(newX, y, newZ);
        stair.gameObject.SetActive(true);
        stair.behavior = ArmBehaviour.ArmBehaviorType.MovementPlayback;
        stair.GetComponent<PivotPoint>().pivotPointType = PivotPoint.PivotPointType.ShoulderNoRotation;
      }
    }
  }

  void Awake()
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
      var arm = position.x < 0.0 ? Instantiate(leftArmPrefab) : Instantiate(rightArmPrefab);
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