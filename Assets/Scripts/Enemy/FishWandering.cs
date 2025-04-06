using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishWandering : MonoBehaviour
{

    public float wanderCellSize = 80;
    [Range(0, 1)]
    public float wanderCellPadding = 0.7f;

    public Vector3 targetPos;

    void Start()
    {
        transform.position = GetRandomPoint();
        GetNewPoint();
    }

    private const float TARGET_THRESHOLD = 1;
    private const float CHECK_RATE = 0.5f;
    float checkTimer;
    void FixedUpdate()
    {
        if (checkTimer <= 0)
        {
            if (Vector3.SqrMagnitude(transform.position - targetPos) <= TARGET_THRESHOLD * TARGET_THRESHOLD)
            {
                GetNewPoint();
            }
            checkTimer += CHECK_RATE;
        }
        checkTimer -= Time.fixedDeltaTime;


    }

    public void GetNewPoint()
    {
        targetPos = GetRandomAdjacentPoint();
    }

    Vector3 GetRandomPoint(Vector3 cell)
    {
        Vector3 resultPosition = cell * wanderCellSize;
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding,
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding,
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding
        );

        resultPosition += randomOffset;

        //Debug.Log("Offset by " + randomOffset + " result position is " + resultPosition);

        return resultPosition;
    }

    Vector3 GetRandomPoint()
    {
        Vector3 cell = transform.position / wanderCellSize;
        cell = new Vector3(Mathf.Round(cell.x), Mathf.Round(cell.y), Mathf.Round(cell.z));

        return GetRandomPoint(cell);
    }

    Vector3 GetRandomAdjacentPoint()
    {
        Vector3 cell = transform.position / wanderCellSize;
        cell = new Vector3(Mathf.Round(cell.x), Mathf.Round(cell.y), Mathf.Round(cell.z));

        Vector3 adjacentCell = cell;

        switch (Random.Range(0, 6))
        {
            case 0: adjacentCell.x++; break;
            case 1: adjacentCell.x--; break;
            case 2: adjacentCell.y++; break;
            case 3: adjacentCell.y--; break;
            case 4: adjacentCell.z++; break;
            case 5: adjacentCell.z--; break;
        }

        //Debug.Log("Started at " + cell + " and moving to cell " + adjacentCell);

        return GetRandomPoint(adjacentCell);


    }

}
