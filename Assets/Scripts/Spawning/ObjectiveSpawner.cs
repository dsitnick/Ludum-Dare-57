using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour
{

    public InfinityGenerator infinityGenerator;
    public float minDistance = 320, maxDistance = 800;

    public GameObject objectiveObject;

    void Awake()
    {
        SpawnObjective();
    }

    public void SpawnObjective()
    {
        Vector3 direction = Random.onUnitSphere * Random.Range(minDistance, maxDistance);
        
        if (direction.y > 0) direction.y *= -1;
        Vector3Int coordinate = infinityGenerator.GetCoordinate(transform.position + direction);
        Vector3 pos = infinityGenerator.GetCoordinatePosition(coordinate);

        PipeObjective obj = objectiveObject.GetComponent<PipeObjective>();
        obj.Setup(pos, infinityGenerator.scale);
    }

}
