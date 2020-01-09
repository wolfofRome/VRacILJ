using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CarCheckpointScript : MonoBehaviour
{
    [System.NonSerialized]
    public int totalCheckpointsPassed;
    [System.NonSerialized]
    public int positionInRace;
    private int currentLap;
    private int currentCheckpointNum;
    private Transform nextCheckpoint;
    private Transform lastCheckpoint;

    public Text lapCountText;

    // Start is called before the first frame update
    void Start()
    {
        totalCheckpointsPassed = 0;
        currentLap = 1;
        lapCountText.text = "Lap\n" + currentLap + " / 3";
        currentCheckpointNum = 0;
        nextCheckpoint = LapSystem.Instance.FirstCheckpoint();
        lastCheckpoint = LapSystem.Instance.LastCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Respawn"))
        {
			Respawn();
        }
    }

	public void Respawn() 
	{
		if(gameObject.tag == "Player") 
		{
			transform.position = lastCheckpoint.position;
			transform.forward = lastCheckpoint.forward;
		}
	}

	public Transform GetNextCheckPoint() 
	{
		return nextCheckpoint;
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
            lapCountText.text = "Lap\n" + currentLap + " / 3";
            Debug.Log("Lap finished : current lap is : " + currentLap);
            if(gameObject.tag == "Player" && currentLap == 4)
            {
                CrossSceneInformation.PlayerHasFinishedGame = true;
                CrossSceneInformation.PlayerFinalPositionInRace = positionInRace;
                CrossSceneInformation.TimeToFinishRace = GameManagerTimer.GetInstance().GetTimerAsString();
                SceneManager.LoadScene("Menu");
            }
        }
        lastCheckpoint = nextCheckpoint;
        nextCheckpoint = LapSystem.Instance.NextCheckpoint(ref currentCheckpointNum);
        ++totalCheckpointsPassed;
    }
}
