using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityGenerator : MonoBehaviour
{

    public Transform root;
    public Transform playerRoot;
    public GameObject prefab;
    public float scale;
    public int buildGridSize;
    public float clearDistance;

    private Dictionary<Vector3Int, GameObject> spawnedItems = new Dictionary<Vector3Int, GameObject>();
    private Queue<GameObject> inactiveItems = new Queue<GameObject>();

    private Vector3Int lastCoordinate;

    void Start()
    {
        Refresh(true);
    }

    void FixedUpdate()
    {
        Refresh(false);
    }

    void Refresh(bool force)
    {
        Vector3Int coordinate = GetCoordinate(playerRoot.position);
        if (!force && coordinate == lastCoordinate) return;

        RefreshBuild();
        RefreshClear();

        lastCoordinate = coordinate;
    }

    void RefreshBuild()
    {

        Vector3Int coordinate = GetCoordinate(playerRoot.position);
        for (int x = -buildGridSize; x <= buildGridSize; x++)
        {
            for (int y = -buildGridSize; y <= buildGridSize; y++)
            {
                for (int z = -buildGridSize; z <= buildGridSize; z++)
                {
                    Build(coordinate + new Vector3Int(x, y, z));
                }
            }
        }

    }

    void RefreshClear()
    {

        foreach (Vector3Int coordinate in new List<Vector3Int>(spawnedItems.Keys))
        {
            Vector3 position = GetCoordinatePosition(coordinate);

            float dist = clearDistance * scale;

            if (Vector3.SqrMagnitude(position - playerRoot.position) >= dist * dist)
            {
                GameObject item = spawnedItems[coordinate];
                inactiveItems.Enqueue(item);
                spawnedItems.Remove(coordinate);
                //Destroy(item);
                item.SetActive(false);
            }
        }
    }

    Vector3Int GetCoordinate(Vector3 worldPosition)
    {
        Vector3 localPosition = root.InverseTransformPoint(worldPosition);
        localPosition /= scale;
        return new Vector3Int(
            Mathf.FloorToInt(localPosition.x),
            Mathf.FloorToInt(localPosition.y),
            Mathf.FloorToInt(localPosition.z)
        );
    }

    Vector3 GetCoordinatePosition(Vector3Int coordinate)
    {
        Vector3 pos = coordinate;
        pos *= scale;
        return root.TransformPoint(pos);
    }

    void Build(Vector3Int coordinate)
    {
        if (spawnedItems.ContainsKey(coordinate)) return;

        GameObject item;
        if (inactiveItems.Count == 0)
        {
            item = Instantiate(prefab, root);
        }
        else
        {
            item = inactiveItems.Dequeue();
            item.SetActive(true);
        }

        item.transform.localPosition = (Vector3)coordinate * scale;
        spawnedItems[coordinate] = item;
    }


}
