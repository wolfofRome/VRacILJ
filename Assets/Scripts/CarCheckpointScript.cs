using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckpointScript : MonoBehaviour
{
    private int currentLap;
    private int currentCheckpointNum;
    private Transform nextCheckpoint;
    private Transform lastCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        currentLap = 1;
        currentCheckpointNum = 0;
        nextCheckpoint = LapSystem.Instance.FirstCheckpoint();
        lastCheckpoint = LapSystem.Instance.LastCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Respawn"))
        {
            transform.position = lastCheckpoint.position;
        }
    }

    public bool IsNextCheckpoint(Transform checkpoint)
    {
        return nextCheckpoint == checkpoint;
    }

    public void NextCheckpoint()
    {
        if(currentCheckpointNum == LapSystem.Instance.checkpointsList.Count - 1)
        {
            currentLap++;
            Debug.Log("Lap finished : current lap is : " + currentLap);
        }
        lastCheckpoint = nextCheckpoint;
        nextCheckpoint = LapSystem.Instance.NextCheckpoint(ref currentCheckpointNum);
    }
}
