using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapSystem : MonoBehaviour
{
    private static LapSystem instance;
    public static LapSystem Instance
    {
        get { return instance; }
    }
    public int maxLaps = 3;
    public List<Transform> checkpointsList;

    private void Awake() {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        checkpointsList = new List<Transform>();
        foreach(CheckpointScript checkpoint in GetComponentsInChildren<CheckpointScript>())
            checkpointsList.Add(checkpoint.transform);
    }

    public Transform FirstCheckpoint()
    {
        return checkpointsList[0];
    }

    public Transform LastCheckpoint()
    {
        return checkpointsList[checkpointsList.Count-1];
    }

    public Transform NextCheckpoint(ref int indexCurrentCheckpoint)
    {
        indexCurrentCheckpoint = (indexCurrentCheckpoint + 1) % checkpointsList.Count;
        return checkpointsList[indexCurrentCheckpoint];
    }
}
