using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public TextMeshProUGUI PositionInRaceText;
	public TextMeshProUGUI RaceTimeText;
	public void Start()
	{
		if(CrossSceneInformation.PlayerHasFinishedGame)
		{
			int playerPos = CrossSceneInformation.PlayerFinalPositionInRace;
			PositionInRaceText.gameObject.SetActive(true);
			RaceTimeText.gameObject.SetActive(true);
			PositionInRaceText.text = "You finished the race in " + playerPos + ((playerPos==1)?("st"):((playerPos==2)?("nd"):((playerPos==3)?("rd"):("th")))) + " place.";
			RaceTimeText.text = "Total time : " + CrossSceneInformation.TimeToFinishRace;
		}
		else
		{
			PositionInRaceText.gameObject.SetActive(false);
			RaceTimeText.gameObject.SetActive(false);
		}
	}
	public void PlayGame() 
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ExitGame() 
	{
		Application.Quit();
	}
}
