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
    private float timerFreezeTimer;

    public float timeFreezingAfterCheckpoint = 1;
    public Text lapCountText;
    public Text timerText;
    public Text fpsText;
    public AudioSource checkpointSound;

    // Start is called before the first frame update
    void Start()
    {
        totalCheckpointsPassed = 0;
        currentLap = 1;
        if(lapCountText != null)
            lapCountText.text = "Lap\n" + currentLap + " / 3";
        currentCheckpointNum = 0;
        nextCheckpoint = LapSystem.Instance.FirstCheckpoint();
        lastCheckpoint = LapSystem.Instance.LastCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(fpsText != null)
            fpsText.text = Mathf.Ceil(1f / Time.unscaledDeltaTime) + " FPS";
        if (GameManagerTimer.GetInstance() != null && GameManagerTimer.GetInstance().CanVehiclesDrive())
        {
            if(!(timerFreezeTimer > 0) && timerText != null)
            {
                timerText.text = GameManagerTimer.GetInstance().GetTimerAsString();
            }
            else
            {
                timerFreezeTimer -= Time.deltaTime;
            }
        }
        else
        {
            if(timerText != null)
                timerText.text = "00:00:000";
        }
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
            if(lapCountText != null)
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

        if(totalCheckpointsPassed % 5 == 0)
        {
            if(checkpointSound != null)
                checkpointSound.Play();
            timerFreezeTimer = timeFreezingAfterCheckpoint;
        }
    }
}
