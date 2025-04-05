using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishWandering : MonoBehaviour
{

    public float swimSpeed = 1;
    public float velocityLerp = 8, rotationLerp = 8;

    public float wanderCellSize = 80;
    [Range(0, 1)]
    public float wanderCellPadding = 0.7f;


    private const float TARGET_THRESHOLD = 1;
    private Vector3 targetPos;


    void Start()
    {
        targetPos = GetRandomAdjacentPoint();
    }

    void FixedUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - targetPos) <= TARGET_THRESHOLD * TARGET_THRESHOLD)
        {
            targetPos = GetRandomAdjacentPoint();
        }


    }

    Vector3 velocity;
    void Update()
    {
        Vector3 targetDirection = (targetPos - transform.position).normalized;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationLerp * Time.deltaTime);
        velocity = Vector3.Lerp(velocity, transform.forward * swimSpeed, velocityLerp * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
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
        
        Debug.Log("Started at " + cell + " and moving to cell " + adjacentCell);

        Vector3 resultPosition = adjacentCell * wanderCellSize;
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding,
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding,
            Random.Range(-0.5f, 0.5f) * wanderCellSize * wanderCellPadding
        );
        
        resultPosition += randomOffset;
        
        Debug.Log("Offset by " + randomOffset + " result position is " + resultPosition);

        return resultPosition;
    }

}
